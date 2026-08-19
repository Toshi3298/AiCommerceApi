using AiCommerceApi.Features.BiaAgent.PendingActions;
using Microsoft.Extensions.Caching.Memory;

namespace AiCommerceApi.Features.BiaAgent.Memory;

public sealed class InMemoryBiaConversationMemory
    : IBiaConversationMemory
{
    private readonly IMemoryCache _memoryCache;

    private readonly object
        _pendingActionLock = new();

    private static readonly TimeSpan
        ConversationLifetime =
            TimeSpan.FromMinutes(30);

    private static readonly TimeSpan
        PendingActionLifetime =
            TimeSpan.FromMinutes(5);

    public InMemoryBiaConversationMemory(
        IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void SaveProductIds(
        Guid conversationId,
        IEnumerable<int> productIds)
    {
        if (conversationId == Guid.Empty)
        {
            return;
        }

        int[] ids = productIds
            .Where(id => id > 0)
            .Distinct()
            .ToArray();

        if (ids.Length == 0)
        {
            return;
        }

        _memoryCache.Set(
            CreateProductIdsCacheKey(
                conversationId),
            ids,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration =
                    ConversationLifetime
            });
    }

    public void SaveCurrentProductId(
        Guid conversationId,
        int productId)
    {
        if (conversationId == Guid.Empty ||
            productId <= 0)
        {
            return;
        }

        _memoryCache.Set(
            CreateCurrentProductCacheKey(
                conversationId),
            productId,
            new MemoryCacheEntryOptions
            {
                SlidingExpiration =
                    ConversationLifetime
            });
    }

    public bool TryGetCurrentProductId(
        Guid conversationId,
        out int productId)
    {
        if (conversationId == Guid.Empty)
        {
            productId = 0;
            return false;
        }

        bool found =
            _memoryCache.TryGetValue(
                CreateCurrentProductCacheKey(
                    conversationId),
                out int storedProductId);

        if (!found || storedProductId <= 0)
        {
            productId = 0;
            return false;
        }

        productId = storedProductId;
        return true;
    }


    public bool TryGetProductIds(
        Guid conversationId,
        out IReadOnlyList<int> productIds)
    {
        if (conversationId == Guid.Empty)
        {
            productIds = Array.Empty<int>();
            return false;
        }

        bool found =
            _memoryCache.TryGetValue(
                CreateProductIdsCacheKey(
                    conversationId),
                out int[]? storedProductIds);

        if (!found ||
            storedProductIds is null ||
            storedProductIds.Length == 0)
        {
            productIds = Array.Empty<int>();
            return false;
        }

        productIds = storedProductIds;
        return true;
    }

    public void SavePendingAction(
        Guid conversationId,
        BiaPendingAction pendingAction)
    {
        if (conversationId == Guid.Empty)
        {
            return;
        }

        lock (_pendingActionLock)
        {
            _memoryCache.Set(
                CreatePendingActionCacheKey(
                    conversationId),
                pendingAction,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        PendingActionLifetime
                });
        }
    }

    public bool TryGetPendingAction(
        Guid conversationId,
        out BiaPendingAction? pendingAction)
    {
        if (conversationId == Guid.Empty)
        {
            pendingAction = null;
            return false;
        }

        lock (_pendingActionLock)
        {
            return _memoryCache.TryGetValue(
                CreatePendingActionCacheKey(
                    conversationId),
                out pendingAction);
        }
    }

    public bool TryTakePendingAction(
        Guid conversationId,
        int userId,
        out BiaPendingAction? pendingAction)
    {
        if (conversationId == Guid.Empty ||
            userId <= 0)
        {
            pendingAction = null;
            return false;
        }

        lock (_pendingActionLock)
        {
            string cacheKey =
                CreatePendingActionCacheKey(
                    conversationId);

            bool found =
                _memoryCache.TryGetValue(
                    cacheKey,
                    out pendingAction);

            if (!found ||
                pendingAction is null ||
                pendingAction.UserId != userId)
            {
                pendingAction = null;
                return false;
            }

            _memoryCache.Remove(cacheKey);

            return true;
        }
    }

    public void ClearPendingAction(
        Guid conversationId)
    {
        if (conversationId == Guid.Empty)
        {
            return;
        }

        lock (_pendingActionLock)
        {
            _memoryCache.Remove(
                CreatePendingActionCacheKey(
                    conversationId));
        }
    }

    private static string
        CreateProductIdsCacheKey(
            Guid conversationId)
    {
        return
            $"bia-conversation:{conversationId}:products";
    }

    private static string
        CreatePendingActionCacheKey(
            Guid conversationId)
    {
        return
            $"bia-conversation:{conversationId}:pending";
    }
    private static string
        CreateCurrentProductCacheKey(
            Guid conversationId)
    {
        return
            $"bia-conversation:{conversationId}:current-product";
    }
}