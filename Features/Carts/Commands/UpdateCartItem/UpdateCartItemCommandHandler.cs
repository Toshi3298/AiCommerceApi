using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler
    : IRequestHandler<
        UpdateCartItemCommand,
        UpdateCartItemResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateCartItemCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCartItemResult> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            return new UpdateCartItemResult
            {
                Success = false,
                Error = "Ürün miktarı sıfırdan büyük olmalıdır."
            };
        }

        var cartItem = await _context.CartItems
            .Include(item => item.Cart)
            .Include(item => item.Product)
            .FirstOrDefaultAsync(
                item =>
                    item.Id == request.CartItemId &&
                    item.Cart.AppUserId == request.UserId,
                cancellationToken);

        if (cartItem is null)
        {
            return new UpdateCartItemResult
            {
                Success = false,
                NotFound = true,
                Error = "Sepet ürünü bulunamadı."
            };
        }

        if (!cartItem.Product.IsActive)
        {
            return new UpdateCartItemResult
            {
                Success = false,
                Error = "Ürün artık satışta değil."
            };
        }

        if (request.Quantity > cartItem.Product.Stock)
        {
            return new UpdateCartItemResult
            {
                Success = false,
                Error =
                    $"Yeterli stok bulunmuyor. Mevcut stok: " +
                    $"{cartItem.Product.Stock}"
            };
        }

        cartItem.Quantity = request.Quantity;
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateCartItemResult
        {
            Success = true,
            Quantity = cartItem.Quantity,
            LineTotal =
                cartItem.Product.Price * cartItem.Quantity
        };
    }
}