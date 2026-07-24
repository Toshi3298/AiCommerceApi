namespace AiCommerceApi.Features.Carts.Commands.ClearCart;

public class ClearCartResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public int RemovedItemCount { get; set; }
}