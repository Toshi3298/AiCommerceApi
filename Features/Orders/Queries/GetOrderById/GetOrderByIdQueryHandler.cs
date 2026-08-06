using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler
    : IRequestHandler<
        GetOrderByIdQuery,
        GetOrderByIdResult>
{
    private readonly ApplicationDbContext _context;

    public GetOrderByIdQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetOrderByIdResult> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(order =>
                order.Id == request.OrderId &&
                order.AppUserId == request.UserId)
            .Select(order => new OrderDetailResponseDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                ShippingAddress = order.ShippingAddress,
                TotalPrice = order.TotalPrice,

                Items = order.OrderItems
                    .Select(item => new OrderItemResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.Name,
                        ImageUrl = item.Product.ImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        LineTotal =
                            item.UnitPrice * item.Quantity
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return new GetOrderByIdResult
            {
                Success = false,
                NotFound = true,
                Error = "Sipariş bulunamadı."
            };
        }

        return new GetOrderByIdResult
        {
            Success = true,
            Order = order
        };
    }
}