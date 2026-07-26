using AiCommerceApi.Dtos.Orders;

namespace AiCommerceApi.Features.Orders.Queries.GetOrders;

public class GetOrdersResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public List<OrderSummaryResponseDto> Orders { get; set; }
        = new();
}