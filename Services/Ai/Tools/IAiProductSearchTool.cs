using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Services.Ai.Tools;

public interface IAiProductSearchTool
{
    Task<List<ProductDto>> SearchAsync(
        AiProductSearchFilterDto filter,
        CancellationToken cancellationToken);
}