using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Memory;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .GetPreviousProductDetails;

public sealed class
    GetPreviousProductDetailsActionHandler
    : IBiaAgentActionHandler
{
    private readonly IBiaConversationMemory
        _conversationMemory;

    private readonly IAiProductDetailsTool
        _productDetailsTool;

    public GetPreviousProductDetailsActionHandler(
        IBiaConversationMemory conversationMemory,
        IAiProductDetailsTool productDetailsTool)
    {
        _conversationMemory = conversationMemory;
        _productDetailsTool = productDetailsTool;
    }

    public string Action =>
        BiaAgentActions
            .GetPreviousProductDetails;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        bool contextFound =
            _conversationMemory.TryGetProductIds(
                context.ConversationId,
                out IReadOnlyList<int> productIds);

        if (!contextFound)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    "Önce bir ürün araması yapmalısın. " +
                    "Ardından listedeki bir ürünün " +
                    "detaylarını sorabilirsin."
            };
        }

        int selectedIndex;

        if (context.Plan.IsLast)
        {
            selectedIndex =
                productIds.Count - 1;
        }
        else
        {
            int position =
                context.Plan.ReferencePosition ?? 0;

            selectedIndex =
                position - 1;
        }

        if (selectedIndex < 0 ||
            selectedIndex >= productIds.Count)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    $"Önceki listede yalnızca " +
                    $"{productIds.Count} ürün bulunuyor."
            };
        }

        int selectedProductId =
            productIds[selectedIndex];

        ProductDto? product =
            await _productDetailsTool.GetByIdAsync(
                selectedProductId,
                cancellationToken);

        if (product is null)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    "Seçtiğin ürünün detayları artık " +
                    "bulunamıyor."
            };
        }
        _conversationMemory.SaveCurrentProductId(
            context.ConversationId,
            product.Id);

        return new BiaChatResponseDto
        {
            Action = Action,

            Message =
                $"{product.Name} ürününün " +
                "detaylarını buldum.",

            Product = product
        };
    }
}