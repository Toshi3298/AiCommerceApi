using System.Security.Claims;
using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Orders;
using AiCommerceApi.Features.Orders.Commands.CreateOrder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Orders.Queries.GetOrders;
using AiCommerceApi.Features.Orders.Queries.GetOrderById;

namespace AiCommerceApi.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderRequestDto request,
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

        var command = new CreateOrderCommand(
            userId,
            request.ShippingAddress);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Sipariş oluşturulamadı.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    orderId = result.OrderId,
                    totalPrice = result.TotalPrice
                },
                "Sipariş başarıyla oluşturuldu.");

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }
    [HttpGet]
    public async Task<IActionResult> GetOrders(
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
            new GetOrdersQuery(userId),
            cancellationToken);

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Siparişler getirilemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                result.Orders,
                "Siparişler başarıyla getirildi.");

        return Ok(response);
    }
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(
        int orderId,
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
            new GetOrderByIdQuery(userId, orderId),
            cancellationToken);

        if (result.NotFound)
        {
            var notFoundResponse =
                ApiResponse<object?>.Fail(
                    result.Error ?? "Sipariş bulunamadı.");

            return NotFound(notFoundResponse);
        }

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Sipariş getirilemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                result.Order,
                "Sipariş başarıyla getirildi.");

        return Ok(response);
    }
}