using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithAi;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
}