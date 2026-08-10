using AiCommerceApi.Common.Pagination;
using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Admin.Orders;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrders;

public class GetAdminOrdersQueryHandler
    : IRequestHandler<
        GetAdminOrdersQuery,
        PagedResult<AdminOrderSummaryDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAdminOrdersQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<AdminOrderSummaryDto>> Handle(
        GetAdminOrdersQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Order> query = _context.Orders
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string search = request.Search.Trim();

            query = query.Where(order =>
                order.AppUser.FirstName.Contains(search) ||
                order.AppUser.LastName.Contains(search) ||
                order.AppUser.Email.Contains(search) ||
                order.Id.ToString().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<OrderStatus>(
                request.Status,
                ignoreCase: true,
                out var orderStatus))
        {
            query = query.Where(order =>
                order.Status == orderStatus);
        }

        int totalCount = await query.CountAsync(
            cancellationToken);

        var orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
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

        return new PagedResult<AdminOrderSummaryDto>
        {
            Items = orders,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            TotalPages = (int)Math.Ceiling(
                totalCount / (double)request.PageSize)
        };
    }
}