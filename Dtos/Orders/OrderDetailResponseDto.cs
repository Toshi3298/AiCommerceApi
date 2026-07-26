namespace AiCommerceApi.Dtos.Orders;

public class OrderDetailResponseDto
{
    public int OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public List<OrderItemResponseDto> Items { get; set; }
        = new();
}