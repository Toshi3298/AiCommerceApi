using MediatR;

namespace AiCommerceApi.Features.Carts.Commands.ClearCart;

public record ClearCartCommand(
    int UserId
) : IRequest<ClearCartResult>;