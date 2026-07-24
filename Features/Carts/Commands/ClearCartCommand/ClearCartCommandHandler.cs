using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Carts.Commands.ClearCart;

public class ClearCartCommandHandler
    : IRequestHandler<ClearCartCommand, ClearCartResult>
{
    private readonly ApplicationDbContext _context;

    public ClearCartCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClearCartResult> Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(
                cart => cart.AppUserId == request.UserId,
                cancellationToken);

        if (cart is null || cart.CartItems.Count == 0)
        {
            return new ClearCartResult
            {
                Success = true,
                RemovedItemCount = 0
            };
        }

        int removedItemCount = cart.CartItems.Count;

        _context.CartItems.RemoveRange(cart.CartItems);

        cart.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ClearCartResult
        {
            Success = true,
            RemovedItemCount = removedItemCount
        };
    }
}