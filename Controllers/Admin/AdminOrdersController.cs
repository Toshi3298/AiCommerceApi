using AiCommerceApi.Common.Responses;
using AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}