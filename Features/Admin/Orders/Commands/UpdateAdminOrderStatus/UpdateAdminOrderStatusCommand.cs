using MediatR;

namespace AiCommerceApi.Features.Admin.Orders.Commands
    .UpdateAdminOrderStatus;

public record UpdateAdminOrderStatusCommand(
    int OrderId,
    string Status
) : IRequest<UpdateAdminOrderStatusResult>;