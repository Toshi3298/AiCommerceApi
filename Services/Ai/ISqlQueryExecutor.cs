using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Services.Ai;

public interface ISqlQueryExecutor
{
    Task<List<ProductDto>> ExecuteProductQueryAsync(
        string sql,
        CancellationToken cancellationToken);
}