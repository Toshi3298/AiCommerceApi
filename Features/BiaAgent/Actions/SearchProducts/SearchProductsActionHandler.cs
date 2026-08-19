using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Services.Ai.Filters;
using AiCommerceApi.Services.Ai.Tools;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .SearchProducts;

public sealed class SearchProductsActionHandler
    : IBiaAgentActionHandler
{
    private readonly IAiProductFilterInterpreter
        _filterInterpreter;

    private readonly IAiProductSearchTool
        _productSearchTool;

    public SearchProductsActionHandler(
        IAiProductFilterInterpreter filterInterpreter,
        IAiProductSearchTool productSearchTool)
    {
        _filterInterpreter = filterInterpreter;
        _productSearchTool = productSearchTool;
    }

    public string Action =>
        BiaAgentActions.SearchProducts;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        AiProductSearchFilterDto filter =
            await _filterInterpreter.InterpretAsync(
                context.Message,
                cancellationToken);

        List<ProductDto> products =
            await _productSearchTool.SearchAsync(
                filter,
                cancellationToken);

        string responseMessage =
            products.Count > 0
                ? "İsteğine uygun ürünleri buldum."
                : "Aradığın kriterlere uygun ürün " +
                  "bulamadım.";

        return new BiaChatResponseDto
        {
            Action = Action,
            Message = responseMessage,
            Products = products
        };
    }
}