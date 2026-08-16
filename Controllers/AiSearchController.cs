using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithAi;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.AiSearch.Queries
    .InterpretProductFilter;
using AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithFilter;

namespace AiCommerceApi.Controllers;

[ApiController]
[Route("api/ai-search")]
public sealed class AiSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public AiSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> SearchProducts(
        AiSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SearchProductsWithAiQuery(request.Prompt),
            cancellationToken);

        var response = ApiResponse<AiSearchResponseDto>.Ok(
            result,
            "AI ürün araması başarıyla işlendi.");

        return Ok(response);
    }
    [HttpPost("interpret")]
    public async Task<IActionResult> InterpretProductFilter(
        AiSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var filter = await _mediator.Send(
            new InterpretProductFilterQuery(
                request.Prompt),
            cancellationToken);

        var response =
            ApiResponse<AiProductSearchFilterDto>.Ok(
                filter,
                "AI ürün arama filtreleri başarıyla yorumlandı.");

        return Ok(response);
    }
    [HttpPost("filter-search")]
    public async Task<IActionResult> SearchProductsWithFilter(
        AiSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new SearchProductsWithFilterQuery(
                request.Prompt),
            cancellationToken);

        var response =
            ApiResponse<AiFilterSearchResponseDto>.Ok(
                result,
                "AI filtre tabanlı ürün araması başarıyla işlendi.");

        return Ok(response);
    }
}