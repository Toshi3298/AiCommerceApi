namespace AiCommerceApi.Dtos.Carts;

public class CartItemResponseDto
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public int AvailableStock { get; set; }

    public bool IsActive { get; set; }
}