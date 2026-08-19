using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Planning;

namespace AiCommerceApi.Features.BiaAgent.Resolution;

public interface IBiaPlanProductResolver
{
    Task<ProductDto?> ResolveAsync(
        BiaAgentPlanDto plan,
        Guid conversationId,
        CancellationToken cancellationToken);
}