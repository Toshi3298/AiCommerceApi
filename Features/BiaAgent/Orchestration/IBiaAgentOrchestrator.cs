using AiCommerceApi.Features.BiaAgent.Chat;

namespace AiCommerceApi.Features.BiaAgent.Orchestration;

public interface IBiaAgentOrchestrator
{
    Task<BiaChatResponseDto> ChatAsync(
        string message,
        CancellationToken cancellationToken);
}