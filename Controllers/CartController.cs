using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Carts;
using AiCommerceApi.Features.Carts.Commands.AddCartItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Carts.Queries.GetCart;

namespace AiCommerceApi.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<IActionResult> GetCart(
        CancellationToken cancellationToken)
    {
        string? userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int userId))
        {
            var unauthorizedResponse =
                ApiResponse<object?>.Fail(
                    "Geçersiz kullanıcı bilgisi.");

            return Unauthorized(unauthorizedResponse);
        }

        var result = await _mediator.Send(
            new GetCartQuery(userId),
            cancellationToken);

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Sepet getirilemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                result.Cart,
                "Sepet başarıyla getirildi.");

        return Ok(response);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem(
        AddCartItemRequestDto request,
        CancellationToken cancellationToken)
    {
        string? userIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdValue, out int userId))
        {
            var unauthorizedResponse =
                ApiResponse<object?>.Fail(
                    "Geçersiz kullanıcı bilgisi.");

            return Unauthorized(unauthorizedResponse);
        }

        var command = new AddCartItemCommand(
            userId,
            request.ProductId,
            request.Quantity);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün sepete eklenemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    cartItemId = result.CartItemId,
                    quantity = result.Quantity
                },
                "Ürün sepete başarıyla eklendi.");

        return Ok(response);
    }
}