using AiCommerceApi.Features.Carts.Commands.AddCartItem;
using MediatR;

namespace AiCommerceApi.Features.BiaAgent.Tools;

public sealed class MediatRBiaCartTool
    : IBiaCartTool
{
    private readonly ISender _sender;

    public MediatRBiaCartTool(
        ISender sender)
    {
        _sender = sender;
    }

    public async Task<AddCartItemResult> AddItemAsync(
        int userId,
        int productId,
        int quantity,
        CancellationToken cancellationToken)
    {
        var command =
            new AddCartItemCommand(
                userId,
                productId,
                quantity);

        return await _sender.Send(
            command,
            cancellationToken);
    }
}