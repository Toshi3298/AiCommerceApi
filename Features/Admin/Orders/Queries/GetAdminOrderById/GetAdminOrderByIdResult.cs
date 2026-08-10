using AiCommerceApi.Dtos.Admin.Orders;

namespace AiCommerceApi.Features.Admin.Orders.Queries
    .GetAdminOrderById;

public class GetAdminOrderByIdResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public AdminOrderDetailDto? Order { get; set; }

    public string? Error { get; set; }
}