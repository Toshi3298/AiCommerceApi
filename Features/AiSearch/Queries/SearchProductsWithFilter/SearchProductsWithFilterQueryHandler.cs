using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Services.Ai.Filters;
using AiCommerceApi.Services.Ai.Tools;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithFilter;

public sealed class SearchProductsWithFilterQueryHandler
    : IRequestHandler<
        SearchProductsWithFilterQuery,
        AiFilterSearchResponseDto>
{
    private readonly IAiProductFilterInterpreter
        _filterInterpreter;

    private readonly IAiProductSearchTool
        _productSearchTool;

    public SearchProductsWithFilterQueryHandler(
        IAiProductFilterInterpreter filterInterpreter,
        IAiProductSearchTool productSearchTool)
    {
        _filterInterpreter = filterInterpreter;
        _productSearchTool = productSearchTool;
    }

    public async Task<AiFilterSearchResponseDto> Handle(
        SearchProductsWithFilterQuery request,
        CancellationToken cancellationToken)
    {
        string prompt = request.Prompt.Trim();

        var filter =
            await _filterInterpreter.InterpretAsync(
                prompt,
                cancellationToken);

        var products =
            await _productSearchTool.SearchAsync(
                filter,
                cancellationToken);

        return new AiFilterSearchResponseDto
        {
            Prompt = prompt,
            Filter = filter,
            Products = products
        };
    }
}