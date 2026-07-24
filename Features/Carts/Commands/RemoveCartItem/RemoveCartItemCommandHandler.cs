using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler
    : IRequestHandler<
        RemoveCartItemCommand,
        RemoveCartItemResult>
{
    private readonly ApplicationDbContext _context;

    public RemoveCartItemCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RemoveCartItemResult> Handle(
        RemoveCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var cartItem = await _context.CartItems
            .Include(item => item.Cart)
            .FirstOrDefaultAsync(
                item =>
                    item.Id == request.CartItemId &&
                    item.Cart.AppUserId == request.UserId,
                cancellationToken);

        if (cartItem is null)
        {
            return new RemoveCartItemResult
            {
                Success = false,
                NotFound = true,
                Error = "Sepet ürünü bulunamadı."
            };
        }

        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync(cancellationToken);

        return new RemoveCartItemResult
        {
            Success = true
        };
    }
}