using AiCommerceApi.Features.BiaAgent.Orchestration;
using MediatR;

namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed class BiaChatQueryHandler
    : IRequestHandler<
        BiaChatQuery,
        BiaChatResponseDto>
{
    private readonly IBiaAgentOrchestrator
        _orchestrator;

    public BiaChatQueryHandler(
        IBiaAgentOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public async Task<BiaChatResponseDto> Handle(
        BiaChatQuery request,
        CancellationToken cancellationToken)
    {
        return await _orchestrator.ChatAsync(
            request.Message.Trim(),
            cancellationToken);
    }
}