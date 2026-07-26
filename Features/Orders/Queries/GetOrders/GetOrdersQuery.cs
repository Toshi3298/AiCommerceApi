using MediatR;

namespace AiCommerceApi.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    int UserId
) : IRequest<GetOrdersResult>;