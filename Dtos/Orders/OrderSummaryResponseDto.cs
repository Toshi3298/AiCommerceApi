namespace AiCommerceApi.Dtos.Orders;

public class OrderSummaryResponseDto
{
    public int OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public int TotalQuantity { get; set; }
}