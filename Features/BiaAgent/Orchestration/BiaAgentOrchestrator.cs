using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;
using AiCommerceApi.Services.Ai.Filters;
using AiCommerceApi.Services.Ai.Tools;

namespace AiCommerceApi.Features.BiaAgent.Orchestration;

public sealed class BiaAgentOrchestrator
    : IBiaAgentOrchestrator
{
    private readonly IBiaActionPlanner
        _actionPlanner;

    private readonly IAiProductFilterInterpreter
        _filterInterpreter;

    private readonly IAiProductSearchTool
        _productSearchTool;

    private readonly IAiProductDetailsTool
        _productDetailsTool;

    public BiaAgentOrchestrator(
        IBiaActionPlanner actionPlanner,
        IAiProductFilterInterpreter filterInterpreter,
        IAiProductSearchTool productSearchTool,
        IAiProductDetailsTool productDetailsTool)
    {
        _actionPlanner = actionPlanner;
        _filterInterpreter = filterInterpreter;
        _productSearchTool = productSearchTool;
        _productDetailsTool = productDetailsTool;
    }

    public async Task<BiaChatResponseDto> ChatAsync(
        string message,
        CancellationToken cancellationToken)
    {
        string normalizedMessage =
            message.Trim();

        BiaAgentPlanDto plan =
            await _actionPlanner.PlanAsync(
                normalizedMessage,
                cancellationToken);

        return plan.Action switch
        {
            BiaAgentActions.SearchProducts =>
                await SearchProductsAsync(
                    normalizedMessage,
                    cancellationToken),

            BiaAgentActions.GetProductDetails =>
                await GetProductDetailsAsync(
                    plan,
                    cancellationToken),

            BiaAgentActions.SearchThenGetDetails =>
                await SearchThenGetDetailsAsync(
                    normalizedMessage,
                    cancellationToken),

            _ => CreateUnsupportedResponse()
        };
    }

    private async Task<BiaChatResponseDto>
        SearchProductsAsync(
            string message,
            CancellationToken cancellationToken)
    {
        AiProductSearchFilterDto filter =
            await _filterInterpreter.InterpretAsync(
                message,
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
            Action =
                BiaAgentActions.SearchProducts,

            Message = responseMessage,
            Products = products
        };
    }

    private async Task<BiaChatResponseDto>
        GetProductDetailsAsync(
            BiaAgentPlanDto plan,
            CancellationToken cancellationToken)
    {
        ProductDto? product = null;

        if (plan.ProductId.HasValue)
        {
            product =
                await _productDetailsTool.GetByIdAsync(
                    plan.ProductId.Value,
                    cancellationToken);
        }
        else if (!string.IsNullOrWhiteSpace(
                     plan.ProductName))
        {
            product =
                await _productDetailsTool.FindByNameAsync(
                    plan.ProductName,
                    cancellationToken);
        }

        if (product is null)
        {
            return new BiaChatResponseDto
            {
                Action =
                    BiaAgentActions.GetProductDetails,

                Message =
                    "İstediğin ürünün detaylarını " +
                    "bulamadım."
            };
        }

        return new BiaChatResponseDto
        {
            Action =
                BiaAgentActions.GetProductDetails,

            Message =
                $"{product.Name} ürününün " +
                "detaylarını buldum.",

            Product = product
        };
    }

    private async Task<BiaChatResponseDto>
        SearchThenGetDetailsAsync(
            string message,
            CancellationToken cancellationToken)
    {
        AiProductSearchFilterDto interpretedFilter =
            await _filterInterpreter.InterpretAsync(
                message,
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
                Action =
                    BiaAgentActions
                        .SearchThenGetDetails,

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
                Action =
                    BiaAgentActions
                        .SearchThenGetDetails,

                Message =
                    "Ürün bulundu ancak detayları " +
                    "getirilemedi."
            };
        }

        return new BiaChatResponseDto
        {
            Action =
                BiaAgentActions
                    .SearchThenGetDetails,

            Message =
                $"{productDetails.Name} ürününün " +
                "detaylarını buldum.",

            Product = productDetails
        };
    }

    private static BiaChatResponseDto
        CreateUnsupportedResponse()
    {
        return new BiaChatResponseDto
        {
            Action =
                BiaAgentActions.Unsupported,

            Message =
                "Şimdilik ürün arama ve ürün detayları " +
                "konularında yardımcı olabilirim."
        };
    }
}