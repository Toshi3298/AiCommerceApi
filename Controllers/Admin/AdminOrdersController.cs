using AiCommerceApi.Common.Responses;
using AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrderById;
using AiCommerceApi.Dtos.Admin.Orders;
using AiCommerceApi.Features.Admin.Orders.Commands.UpdateAdminOrderStatus;

namespace AiCommerceApi.Controllers.Admin;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminOrdersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var orders = await _mediator.Send(
            new GetAdminOrdersQuery(
                search,
                status,
                pageNumber,
                pageSize),
            cancellationToken);

        var response =
            ApiResponse<object?>.Ok(
                orders,
                "Admin sipariş listesi başarıyla getirildi.");

        return Ok(response);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrderById(
        int orderId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetAdminOrderByIdQuery(orderId),
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
                    result.Error ?? "Sipariş getirilemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                result.Order,
                "Admin sipariş detayı başarıyla getirildi.");

        return Ok(response);
    }

    [HttpPut("{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        int orderId,
        UpdateAdminOrderStatusRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateAdminOrderStatusCommand(
                orderId,
                request.Status),
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
                    ?? "Sipariş durumu güncellenemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    orderId = result.OrderId,
                    status = result.Status
                },
                "Sipariş durumu başarıyla güncellendi.");

        return Ok(response);
    }
}