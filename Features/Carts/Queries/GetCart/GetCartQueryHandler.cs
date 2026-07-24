using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Carts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Carts.Queries.GetCart;

public class GetCartQueryHandler
    : IRequestHandler<GetCartQuery, GetCartResult>
{
    private readonly ApplicationDbContext _context;

    public GetCartQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetCartResult> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Include(cart => cart.CartItems)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(
                cart => cart.AppUserId == request.UserId,
                cancellationToken);

        if (cart is null)
        {
            return new GetCartResult
            {
                Success = true,
                Cart = new CartResponseDto()
            };
        }

        var items = cart.CartItems
            .Select(item => new CartItemResponseDto
            {
                CartItemId = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                UnitPrice = item.Product.Price,
                Quantity = item.Quantity,
                LineTotal =
                    item.Product.Price * item.Quantity,
                AvailableStock = item.Product.Stock,
                IsActive = item.Product.IsActive
            })
            .ToList();

        return new GetCartResult
        {
            Success = true,
            Cart = new CartResponseDto
            {
                CartId = cart.Id,
                Items = items,
                TotalQuantity =
                items.Sum(item => item.Quantity),
                TotalPrice =
                items.Sum(item => item.LineTotal)
            }
        };
    }
}