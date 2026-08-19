using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;
using AiCommerceApi.Features.BiaAgent.Memory;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .GetProductDetails;

public sealed class GetProductDetailsActionHandler
    : IBiaAgentActionHandler
{
    private readonly IAiProductDetailsTool
        _productDetailsTool;
    private readonly IBiaConversationMemory
        _conversationMemory;

    public GetProductDetailsActionHandler(
        IAiProductDetailsTool productDetailsTool,
        IBiaConversationMemory conversationMemory)
    {
        _productDetailsTool =
            productDetailsTool;

        _conversationMemory =
            conversationMemory;
    }

    public string Action =>
        BiaAgentActions.GetProductDetails;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        ProductDto? product = null;

        if (context.Plan.ProductId.HasValue)
        {
            product =
                await _productDetailsTool.GetByIdAsync(
                    context.Plan.ProductId.Value,
                    cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(
                     context.Plan.ProductName))
        {
            product =
                await _productDetailsTool.FindByNameAsync(
                    context.Plan.ProductName,
                    cancellationToken);
        }

        if (product is null)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    "İstediğin ürünün detaylarını " +
                    "bulamadım."
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