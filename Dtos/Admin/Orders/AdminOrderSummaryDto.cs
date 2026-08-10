namespace AiCommerceApi.Dtos.Admin.Orders;

public class AdminOrderSummaryDto
{
    public int OrderId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    public int TotalQuantity { get; set; }

    public int UserId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;
}