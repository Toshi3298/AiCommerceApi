using AiCommerceApi.Dtos.Admin.Orders;

namespace AiCommerceApi.Dtos.Admin.Dashboard;

public class AdminDashboardDto
{
    public int TotalProducts { get; set; }

    public int ActiveProducts { get; set; }

    public int OutOfStockProducts { get; set; }

    public int TotalCategories { get; set; }

    public int TotalCustomers { get; set; }

    public int TotalOrders { get; set; }

    public int PendingOrders { get; set; }

    public decimal TotalRevenue { get; set; }

    public List<AdminOrderSummaryDto> RecentOrders { get; set; }
        = new();
}