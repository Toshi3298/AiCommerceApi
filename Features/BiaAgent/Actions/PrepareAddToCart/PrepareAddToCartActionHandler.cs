using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Memory;
using AiCommerceApi.Features.BiaAgent.PendingActions;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Resolution;
using AiCommerceApi.Features.BiaAgent.Actions;

namespace AiCommerceApi.Features.BiaAgent.Actions.PrepareAddToCart;

public sealed class PrepareAddToCartActionHandler
    : IBiaAgentActionHandler
{
    private readonly IBiaPlanProductResolver
        _productResolver;

    private readonly IBiaConversationMemory
        _conversationMemory;

    public PrepareAddToCartActionHandler(
        IBiaPlanProductResolver productResolver,
        IBiaConversationMemory conversationMemory)
    {
        _productResolver = productResolver;
        _conversationMemory = conversationMemory;
    }

    public string Action =>
        BiaAgentActions.PrepareAddToCart;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        if (!context.UserId.HasValue)
        {
            return CreateAuthenticationRequiredResponse(
                context.ConversationId);
        }

        ProductDto? product =
            await _productResolver.ResolveAsync(
                context.Plan,
                context.ConversationId,
                cancellationToken);

        if (product is null)
        {
            return CreateProductNotFoundResponse(
                context.ConversationId);
        }

        int quantity =
            context.Plan.Quantity ?? 1;

        if (quantity <= 0)
        {
            return CreateInvalidQuantityResponse(
                context.ConversationId,
                product);
        }

        if (!product.IsActive)
        {
            return CreateInactiveProductResponse(
                context.ConversationId,
                product);
        }

        if (product.Stock <= 0)
        {
            return CreateOutOfStockResponse(
                context.ConversationId,
                product);
        }

        if (quantity > product.Stock)
        {
            return CreateInsufficientStockResponse(
                context.ConversationId,
                product,
                quantity);
        }

        var pendingAction =
            new BiaPendingAction
            {
                Action =
                    BiaAgentActions.PrepareAddToCart,

                UserId =
                    context.UserId.Value,

                ProductId =
                    product.Id,

                ProductName =
                    product.Name,

                Quantity =
                    quantity,

                CreatedAt =
                    DateTime.UtcNow
            };

        _conversationMemory.SavePendingAction(
            context.ConversationId,
            pendingAction);

        return new BiaChatResponseDto
        {
            ConversationId =
                context.ConversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                $"{product.Name} ürününden " +
                $"{quantity} adet sepete eklensin mi?",

            Product =
                product,

            RequiresConfirmation =
                true,

            RequiresAuthentication =
                false
        };
    }

    private static BiaChatResponseDto
        CreateAuthenticationRequiredResponse(
            Guid conversationId)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                "Sepete ürün ekleyebilmem için " +
                "önce giriş yapmalısın.",

            RequiresAuthentication =
                true,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateProductNotFoundResponse(
            Guid conversationId)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                "Sepete eklemek istediğin ürünü " +
                "bulamadım. Ürün adını veya listedeki " +
                "sırasını belirtebilirsin.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateInvalidQuantityResponse(
            Guid conversationId,
            ProductDto product)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                "Sepete eklenecek ürün adedi " +
                "sıfırdan büyük olmalıdır.",

            Product =
                product,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateInactiveProductResponse(
            Guid conversationId,
            ProductDto product)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                $"{product.Name} ürünü artık " +
                "satışta değil.",

            Product =
                product,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateOutOfStockResponse(
            Guid conversationId,
            ProductDto product)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                $"{product.Name} ürününün stoğu " +
                "bulunmuyor.",

            Product =
                product,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateInsufficientStockResponse(
            Guid conversationId,
            ProductDto product,
            int requestedQuantity)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.PrepareAddToCart,

            Message =
                $"{product.Name} ürününden " +
                $"{requestedQuantity} adet istedin. " +
                $"Mevcut stok: {product.Stock}.",

            Product =
                product,

            RequiresConfirmation =
                false
        };
    }
}