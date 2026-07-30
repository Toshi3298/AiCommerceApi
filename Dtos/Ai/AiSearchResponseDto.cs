using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Dtos.Ai;

public sealed class AiSearchResponseDto
{
    public string Prompt { get; init; } = string.Empty;

    public string GeneratedSql { get; init; } = string.Empty;

    public List<ProductDto> Products { get; init; } = new();
}