using AiCommerceApi.Features.Carts.Commands.AddCartItem;

namespace AiCommerceApi.Features.BiaAgent.Tools;

public interface IBiaCartTool
{
    Task<AddCartItemResult> AddItemAsync(
        int userId,
        int productId,
        int quantity,
        CancellationToken cancellationToken);
}