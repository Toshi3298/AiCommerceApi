using AiCommerceApi.Common.Pagination;
using AiCommerceApi.Dtos.Admin.Orders;
using MediatR;

namespace AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrders;

public record GetAdminOrdersQuery(
    string? Search,
    string? Status,
    int PageNumber,
    int PageSize
) : IRequest<PagedResult<AdminOrderSummaryDto>>;