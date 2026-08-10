using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Admin.Orders;
using AiCommerceApi.Dtos.Orders;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Orders.Queries
    .GetAdminOrderById;

public class GetAdminOrderByIdQueryHandler
    : IRequestHandler<
        GetAdminOrderByIdQuery,
        GetAdminOrderByIdResult>
{
    private readonly ApplicationDbContext _context;

    public GetAdminOrderByIdQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAdminOrderByIdResult> Handle(
        GetAdminOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Where(order =>
                order.Id == request.OrderId)
            .Select(order => new AdminOrderDetailDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TotalPrice = order.TotalPrice,
                ShippingAddress = order.ShippingAddress,

                UserId = order.AppUserId,

                CustomerName =
                    order.AppUser.FirstName + " " +
                    order.AppUser.LastName,

                CustomerEmail = order.AppUser.Email,

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
            return new GetAdminOrderByIdResult
            {
                Success = false,
                NotFound = true,
                Error = "Sipariş bulunamadı."
            };
        }

        return new GetAdminOrderByIdResult
        {
            Success = true,
            Order = order
        };
    }
}