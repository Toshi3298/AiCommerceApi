using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed class BiaChatResponseDto
{
    public string Action { get; init; } =
        string.Empty;

    public string Message { get; init; } =
        string.Empty;

    public List<ProductDto> Products { get; init; } =
        [];

    public ProductDto? Product { get; init; }
}