using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Memory;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;

namespace AiCommerceApi.Features.BiaAgent.Resolution;

public sealed class BiaPlanProductResolver
    : IBiaPlanProductResolver
{
    private readonly IAiProductDetailsTool
        _productDetailsTool;

    private readonly IBiaConversationMemory
        _conversationMemory;

    public BiaPlanProductResolver(
        IAiProductDetailsTool productDetailsTool,
        IBiaConversationMemory conversationMemory)
    {
        _productDetailsTool = productDetailsTool;
        _conversationMemory = conversationMemory;
    }

    public async Task<ProductDto?> ResolveAsync(
        BiaAgentPlanDto plan,
        Guid conversationId,
        CancellationToken cancellationToken)
    {
        int? referencedProductId =
            ResolveReferencedProductId(
                plan,
                conversationId);

        if (referencedProductId.HasValue)
        {
            return await _productDetailsTool.GetByIdAsync(
                referencedProductId.Value,
                cancellationToken);
        }

        if (plan.ProductId.HasValue &&
            plan.ProductId.Value > 0)
        {
            return await _productDetailsTool.GetByIdAsync(
                plan.ProductId.Value,
                cancellationToken);
        }

        if (!string.IsNullOrWhiteSpace(
                plan.ProductName))
        {
            return await _productDetailsTool.FindByNameAsync(
                plan.ProductName.Trim(),
                cancellationToken);
        }

        return null;
    }

    private int? ResolveReferencedProductId(
        BiaAgentPlanDto plan,
        Guid conversationId)
    {
        bool hasPreviousReference =
            plan.IsLast ||
            plan.ReferencePosition.HasValue;

        if (!hasPreviousReference ||
            conversationId == Guid.Empty)
        {
            return null;
        }

        bool found =
            _conversationMemory.TryGetProductIds(
                conversationId,
                out IReadOnlyList<int> productIds);

        if (!found || productIds.Count == 0)
        {
            return null;
        }

        if (plan.IsLast)
        {
            return productIds[^1];
        }

        if (!plan.ReferencePosition.HasValue)
        {
            return null;
        }

        int index =
            plan.ReferencePosition.Value - 1;

        if (index < 0 ||
            index >= productIds.Count)
        {
            return null;
        }

        return productIds[index];
    }
}