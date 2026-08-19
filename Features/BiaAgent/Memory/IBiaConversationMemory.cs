using AiCommerceApi.Features.BiaAgent.PendingActions;

namespace AiCommerceApi.Features.BiaAgent.Memory;

public interface IBiaConversationMemory
{
    void SaveProductIds(
        Guid conversationId,
        IEnumerable<int> productIds);

    bool TryGetProductIds(
        Guid conversationId,
        out IReadOnlyList<int> productIds);

    void SavePendingAction(
        Guid conversationId,
        BiaPendingAction pendingAction);

    void SaveCurrentProductId(
    Guid conversationId,
    int productId);

    bool TryGetCurrentProductId(
        Guid conversationId,
        out int productId);

    bool TryGetPendingAction(
        Guid conversationId,
        out BiaPendingAction? pendingAction);

    bool TryTakePendingAction(
        Guid conversationId,
        int userId,
        out BiaPendingAction? pendingAction);

    void ClearPendingAction(
        Guid conversationId);
}