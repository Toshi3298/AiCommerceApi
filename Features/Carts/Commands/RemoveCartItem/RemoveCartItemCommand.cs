using MediatR;

namespace AiCommerceApi.Features.Carts.Commands.RemoveCartItem;

public record RemoveCartItemCommand(
    int UserId,
    int CartItemId
) : IRequest<RemoveCartItemResult>;