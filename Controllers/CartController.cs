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
using AiCommerceApi.Features.Carts.Commands.UpdateCartItem;
namespace AiCommerceApi.Controllers;
using AiCommerceApi.Features.Carts.Commands.RemoveCartItem;
using AiCommerceApi.Features.Carts.Commands.ClearCart;

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
    [HttpPut("items/{cartItemId:int}")]
public async Task<IActionResult> UpdateCartItem(
    int cartItemId,
    UpdateCartItemRequestDto request,
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

    var command = new UpdateCartItemCommand(
        userId,
        cartItemId,
        request.Quantity);

    var result = await _mediator.Send(
        command,
        cancellationToken);

    if (result.NotFound)
    {
        var notFoundResponse =
            ApiResponse<object?>.Fail(
                result.Error ?? "Sepet ürünü bulunamadı.");

        return NotFound(notFoundResponse);
    }

    if (!result.Success)
    {
        var errorResponse =
            ApiResponse<object?>.Fail(
                result.Error ?? "Miktar güncellenemedi.");

        return BadRequest(errorResponse);
    }

    var response =
        ApiResponse<object?>.Ok(
            new
            {
                cartItemId,
                quantity = result.Quantity,
                lineTotal = result.LineTotal
            },
            "Sepetteki ürün miktarı güncellendi.");

    return Ok(response);
}
[HttpDelete("items/{cartItemId:int}")]
public async Task<IActionResult> RemoveCartItem(
    int cartItemId,
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
        new RemoveCartItemCommand(
            userId,
            cartItemId),
        cancellationToken);

    if (result.NotFound)
    {
        var notFoundResponse =
            ApiResponse<object?>.Fail(
                result.Error ?? "Sepet ürünü bulunamadı.");

        return NotFound(notFoundResponse);
    }

    var response =
        ApiResponse<object?>.Ok(
            null,
            "Ürün sepetten kaldırıldı.");

    return Ok(response);
}
[HttpDelete]
public async Task<IActionResult> ClearCart(
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
        new ClearCartCommand(userId),
        cancellationToken);

    if (!result.Success)
    {
        var errorResponse =
            ApiResponse<object?>.Fail(
                result.Error ?? "Sepet temizlenemedi.");

        return BadRequest(errorResponse);
    }

    var response =
        ApiResponse<object?>.Ok(
            new
            {
                removedItemCount = result.RemovedItemCount
            },
            "Sepet başarıyla temizlendi.");

    return Ok(response);
}
}