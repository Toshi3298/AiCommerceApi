using MediatR;

namespace AiCommerceApi.Features.Carts.Commands.UpdateCartItem;

public record UpdateCartItemCommand(
    int UserId,
    int CartItemId,
    int Quantity
) : IRequest<UpdateCartItemResult>;