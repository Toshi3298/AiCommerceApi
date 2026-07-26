using MediatR;

namespace AiCommerceApi.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(
    int UserId,
    int OrderId
) : IRequest<GetOrderByIdResult>;