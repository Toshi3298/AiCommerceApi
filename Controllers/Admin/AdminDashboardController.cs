using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Admin.Dashboard;
using AiCommerceApi.Features.Admin.Dashboard.Queries
    .GetAdminDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCommerceApi.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = "Admin")]
public class AdminDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminDashboardController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
    {
        var dashboard = await _mediator.Send(
            new GetAdminDashboardQuery(),
            cancellationToken);

        var response =
            ApiResponse<AdminDashboardDto>.Ok(
                dashboard,
                "Admin dashboard bilgileri başarıyla getirildi.");

        return Ok(response);
    }
}