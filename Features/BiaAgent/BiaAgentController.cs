using AiCommerceApi.Common.Responses;
using AiCommerceApi.Features.BiaAgent.Chat;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AiCommerceApi.Features.BiaAgent;

[ApiController]
[Route("api/bia")]
public sealed class BiaAgentController
    : ControllerBase
{
    private readonly IMediator _mediator;

    public BiaAgentController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        BiaChatRequestDto request,
        CancellationToken cancellationToken)
    {
        BiaChatResponseDto result =
            await _mediator.Send(
                new BiaChatQuery(request.Message),
                cancellationToken);

        var response =
            ApiResponse<BiaChatResponseDto>.Ok(
                result,
                "Bia isteği başarıyla işlendi.");

        return Ok(response);
    }
}