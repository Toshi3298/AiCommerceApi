using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;
using AiCommerceApi.Services.Ai.Filters;
using AiCommerceApi.Services.Ai.Tools;
using AiCommerceApi.Features.BiaAgent.Memory;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .SearchThenGetDetails;

public sealed class
    SearchThenGetDetailsActionHandler
    : IBiaAgentActionHandler
{
    private readonly IAiProductFilterInterpreter
        _filterInterpreter;

    private readonly IAiProductSearchTool
        _productSearchTool;

    private readonly IAiProductDetailsTool
        _productDetailsTool;

    private readonly IBiaConversationMemory
        _conversationMemory;

    public SearchThenGetDetailsActionHandler(
        IAiProductFilterInterpreter filterInterpreter,
        IAiProductSearchTool productSearchTool,
        IAiProductDetailsTool productDetailsTool,
        IBiaConversationMemory conversationMemory)
    {
        _filterInterpreter = filterInterpreter;
        _productSearchTool = productSearchTool;
        _productDetailsTool = productDetailsTool;
         _conversationMemory = conversationMemory;
    }

    public string Action =>
        BiaAgentActions.SearchThenGetDetails;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        AiProductSearchFilterDto interpretedFilter =
            await _filterInterpreter.InterpretAsync(
                context.Message,
                cancellationToken);

        var singleProductFilter =
            new AiProductSearchFilterDto
            {
                Search =
                    interpretedFilter.Search,

                Brand =
                    interpretedFilter.Brand,

                CategoryName =
                    interpretedFilter.CategoryName,

                MinPrice =
                    interpretedFilter.MinPrice,

                MaxPrice =
                    interpretedFilter.MaxPrice,

                InStock =
                    interpretedFilter.InStock,

                SortBy =
                    interpretedFilter.SortBy,

                SortDirection =
                    interpretedFilter.SortDirection,

                Limit = 1
            };

        List<ProductDto> products =
            await _productSearchTool.SearchAsync(
                singleProductFilter,
                cancellationToken);

        ProductDto? firstProduct =
            products.FirstOrDefault();

        if (firstProduct is null)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    "Aradığın kriterlere uygun ürün " +
                    "bulamadım."
            };
        }

        ProductDto? productDetails =
            await _productDetailsTool.GetByIdAsync(
                firstProduct.Id,
                cancellationToken);

        if (productDetails is null)
        {
            return new BiaChatResponseDto
            {
                Action = Action,

                Message =
                    "Ürün bulundu ancak detayları " +
                    "getirilemedi."
            };
        }
        _conversationMemory.SaveCurrentProductId(
            context.ConversationId,
            productDetails.Id);
        return new BiaChatResponseDto   
        {
            Action = Action,

            Message =
                $"{productDetails.Name} ürününün " +
                "detaylarını buldum.",

            Product = productDetails
        };
    }
}