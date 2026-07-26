using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler
    : IRequestHandler<GetOrdersQuery, GetOrdersResult>
{
    private readonly ApplicationDbContext _context;

    public GetOrdersQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrdersResult> Handle(
        GetOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Where(order =>
                order.AppUserId == request.UserId)
            .OrderByDescending(order => order.CreatedAt)
            .Select(order => new OrderSummaryResponseDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,
                TotalQuantity = order.OrderItems.Sum(
                    item => item.Quantity)
            })
            .ToListAsync(cancellationToken);

        return new GetOrdersResult
        {
            Success = true,
            Orders = orders
        };
    }
}