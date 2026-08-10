using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Admin.Dashboard;
using AiCommerceApi.Dtos.Admin.Orders;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Dashboard.Queries
    .GetAdminDashboard;

public class GetAdminDashboardQueryHandler
    : IRequestHandler<
        GetAdminDashboardQuery,
        AdminDashboardDto>
{
    private readonly ApplicationDbContext _context;

    public GetAdminDashboardQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardDto> Handle(
        GetAdminDashboardQuery request,
        CancellationToken cancellationToken)
    {
        int totalProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(cancellationToken);

        int activeProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(
                    product => product.IsActive,
                    cancellationToken);

        int outOfStockProducts =
            await _context.Products
                .AsNoTracking()
                .CountAsync(
                    product =>
                        product.IsActive &&
                        product.Stock == 0,
                    cancellationToken);

        int totalCategories =
            await _context.Categories
                .AsNoTracking()
                .CountAsync(cancellationToken);

        int totalCustomers =
            await _context.Users
                .AsNoTracking()
                .CountAsync(
                    user => user.Role == "User",
                    cancellationToken);

        int totalOrders =
            await _context.Orders
                .AsNoTracking()
                .CountAsync(cancellationToken);

        int pendingOrders =
            await _context.Orders
                .AsNoTracking()
                .CountAsync(
                    order =>
                        order.Status == OrderStatus.Pending,
                    cancellationToken);

        decimal totalRevenue =
            await _context.Orders
                .AsNoTracking()
                .Where(order =>
                    order.Status == OrderStatus.Delivered)
                .Select(order => (decimal?)order.TotalPrice)
                .SumAsync(cancellationToken)
            ?? 0;

        var recentOrders =
            await _context.Orders
                .AsNoTracking()
                .OrderByDescending(order => order.CreatedAt)
                .Take(5)
                .Select(order => new AdminOrderSummaryDto
                {
                    OrderId = order.Id,
                    CreatedAt = order.CreatedAt,
                    Status = order.Status.ToString(),
                    TotalPrice = order.TotalPrice,
                    ShippingAddress = order.ShippingAddress,

                    TotalQuantity = order.OrderItems
                        .Sum(item => item.Quantity),

                    UserId = order.AppUserId,

                    CustomerName =
                        order.AppUser.FirstName + " " +
                        order.AppUser.LastName,

                    CustomerEmail = order.AppUser.Email
                })
                .ToListAsync(cancellationToken);

        return new AdminDashboardDto
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            OutOfStockProducts = outOfStockProducts,
            TotalCategories = totalCategories,
            TotalCustomers = totalCustomers,
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TotalRevenue = totalRevenue,
            RecentOrders = recentOrders
        };
    }
}