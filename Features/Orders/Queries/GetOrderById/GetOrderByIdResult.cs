using AiCommerceApi.Dtos.Orders;

namespace AiCommerceApi.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? Error { get; set; }

    public OrderDetailResponseDto? Order { get; set; }
}