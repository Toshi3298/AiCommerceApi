using AiCommerceApi.Features.BiaAgent.Actions;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Planning;

namespace AiCommerceApi.Features.BiaAgent.Orchestration;

public sealed class BiaAgentOrchestrator
    : IBiaAgentOrchestrator
{
    private readonly IBiaActionPlanner
        _actionPlanner;

    private readonly IReadOnlyDictionary<
        string,
        IBiaAgentActionHandler> _actionHandlers;

    public BiaAgentOrchestrator(
        IBiaActionPlanner actionPlanner,
        IEnumerable<IBiaAgentActionHandler>
            actionHandlers)
    {
        _actionPlanner = actionPlanner;

        _actionHandlers =
            actionHandlers.ToDictionary(
                handler => handler.Action,
                StringComparer.OrdinalIgnoreCase);
    }

    public async Task<BiaChatResponseDto> ChatAsync(
        string message,
        Guid conversationId,
        int? userId,
        CancellationToken cancellationToken)
    {
        string normalizedMessage =
            message.Trim();

        BiaAgentPlanDto plan =
            await _actionPlanner.PlanAsync(
                normalizedMessage,
                cancellationToken);

        var context =
            new BiaActionContext(
                normalizedMessage,
                conversationId,
                userId,
                plan);

        if (_actionHandlers.TryGetValue(
                plan.Action,
                out IBiaAgentActionHandler? handler))
        {
            return await handler.HandleAsync(
                context,
                cancellationToken);
        }

        return CreateUnsupportedResponse(
            conversationId);
    }

    private static BiaChatResponseDto
        CreateUnsupportedResponse(
            Guid conversationId)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.Unsupported,

            Message =
                "Şimdilik ürün arama, ürün detayları " +
                "ve sepete ekleme işlemlerinde " +
                "yardımcı olabilirim.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false
        };
    }
}