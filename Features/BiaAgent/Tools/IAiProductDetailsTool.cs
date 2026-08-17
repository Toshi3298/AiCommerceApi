using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Features.BiaAgent.Tools;

public interface IAiProductDetailsTool
{
    Task<ProductDto?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken);

    Task<ProductDto?> FindByNameAsync(
        string productName,
        CancellationToken cancellationToken);
}