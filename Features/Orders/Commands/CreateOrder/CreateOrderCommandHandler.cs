using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<
        CreateOrderCommand,
        CreateOrderResult>
{
    private readonly ApplicationDbContext _context;

    public CreateOrderCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateOrderResult> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(
                cart =>
                    cart.AppUserId == request.UserId,
                cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
        {
            return Failure(
                "Sepetiniz boş olduğu için sipariş oluşturulamadı.");
        }

        foreach (var cartItem in cart.CartItems)
        {
            if (!cartItem.Product.IsActive)
            {
                return Failure(
                    $"{cartItem.Product.Name} artık satışta değil.");
            }

            if (cartItem.Quantity >
                cartItem.Product.Stock)
            {
                return Failure(
                    $"{cartItem.Product.Name} için yeterli stok yok. " +
                    $"Mevcut stok: {cartItem.Product.Stock}");
            }
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
        {
            var order = new Order
            {
                AppUserId = request.UserId,

                ShippingAddress =
                    request.ShippingAddress.Trim(),

                Status = OrderStatus.Pending,

                TotalPrice = cart.CartItems.Sum(
                    item =>
                        item.Product.Price *
                        item.Quantity)
            };

            foreach (var cartItem in cart.CartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Product.Price
                });

                cartItem.Product.Stock -=
                    cartItem.Quantity;
            }

            _context.Orders.Add(order);

            _context.CartItems.RemoveRange(
                cart.CartItems);

            cart.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            return new CreateOrderResult
            {
                Success = true,
                OrderId = order.Id,
                TotalPrice = order.TotalPrice
            };
        }
        catch
        {
            await transaction.RollbackAsync(
                CancellationToken.None);

            throw;
        }
    }

    private static CreateOrderResult Failure(
        string error)
    {
        return new CreateOrderResult
        {
            Success = false,
            Error = error
        };
    }
}