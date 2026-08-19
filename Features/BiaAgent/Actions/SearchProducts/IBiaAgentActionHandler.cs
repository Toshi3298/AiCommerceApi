using AiCommerceApi.Features.BiaAgent.Chat;

namespace AiCommerceApi.Features.BiaAgent.Actions;

public interface IBiaAgentActionHandler
{
    string Action { get; }

    Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken);
}