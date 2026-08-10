using AiCommerceApi.Dtos.Admin.Dashboard;
using MediatR;

namespace AiCommerceApi.Features.Admin.Dashboard.Queries
    .GetAdminDashboard;

public record GetAdminDashboardQuery
    : IRequest<AdminDashboardDto>;