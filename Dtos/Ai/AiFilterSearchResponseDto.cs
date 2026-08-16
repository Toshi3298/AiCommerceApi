using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Dtos.Ai;

public sealed class AiFilterSearchResponseDto
{
    public string Prompt { get; init; } =
        string.Empty;

    public AiProductSearchFilterDto Filter { get; init; } =
        new();

    public List<ProductDto> Products { get; init; } =
        [];
}