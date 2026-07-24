namespace AiCommerceApi.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? Error { get; set; }
}