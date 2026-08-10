namespace AiCommerceApi.Features.Admin.Orders.Commands
    .UpdateAdminOrderStatus;

public class UpdateAdminOrderStatusResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public int OrderId { get; set; }

    public string? Status { get; set; }

    public string? Error { get; set; }
}