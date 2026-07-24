namespace AiCommerceApi.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? Error { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }
}