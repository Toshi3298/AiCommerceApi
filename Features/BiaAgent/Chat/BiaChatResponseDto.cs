using AiCommerceApi.Dtos.Products;

namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed class BiaChatResponseDto
{
    public Guid ConversationId { get; init; }

    public string Action { get; init; } =
        string.Empty;

    public string Message { get; init; } =
        string.Empty;

    public List<ProductDto> Products { get; init; } =
        [];

    public ProductDto? Product { get; init; }

    public bool RequiresAuthentication { get; init; }

    public bool RequiresConfirmation { get; init; }

    public int? CartItemId { get; init; }

    public int? CartQuantity { get; init; }
}