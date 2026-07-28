using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Carts.Commands.AddCartItem;

public class AddCartItemCommandHandler
    : IRequestHandler<
        AddCartItemCommand,
        AddCartItemResult>
{
    private readonly ApplicationDbContext _context;

    public AddCartItemCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AddCartItemResult> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                product =>
                    product.Id == request.ProductId &&
                    product.IsActive,
                cancellationToken);

        if (product is null)
        {
            return Failure(
                "Ürün bulunamadı veya satışta değil.");
        }

        var cart = await _context.Carts
            .Include(cart => cart.CartItems)
            .FirstOrDefaultAsync(
                cart =>
                    cart.AppUserId == request.UserId,
                cancellationToken);

        if (cart is null)
        {
            cart = new Cart
            {
                AppUserId = request.UserId
            };

            _context.Carts.Add(cart);
        }

        var existingCartItem = cart.CartItems
            .FirstOrDefault(
                item =>
                    item.ProductId == request.ProductId);

        int newQuantity = request.Quantity;

        if (existingCartItem is not null)
        {
            newQuantity =
                existingCartItem.Quantity +
                request.Quantity;
        }

        if (newQuantity > product.Stock)
        {
            return Failure(
                $"Yeterli stok bulunmuyor. " +
                $"Mevcut stok: {product.Stock}");
        }

        if (existingCartItem is null)
        {
            existingCartItem = new CartItem
            {
                ProductId = product.Id,
                Quantity = request.Quantity
            };

            cart.CartItems.Add(existingCartItem);
        }
        else
        {
            existingCartItem.Quantity = newQuantity;
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(
            cancellationToken);

        return new AddCartItemResult
        {
            Success = true,
            CartItemId = existingCartItem.Id,
            Quantity = existingCartItem.Quantity
        };
    }

    private static AddCartItemResult Failure(
        string error)
    {
        return new AddCartItemResult
        {
            Success = false,
            Error = error
        };
    }
}