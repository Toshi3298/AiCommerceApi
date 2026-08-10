using MediatR;

namespace AiCommerceApi.Features.Admin.Orders.Queries
    .GetAdminOrderById;

public record GetAdminOrderByIdQuery(
    int OrderId
) : IRequest<GetAdminOrderByIdResult>;