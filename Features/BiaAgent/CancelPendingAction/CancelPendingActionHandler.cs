using AiCommerceApi.Features.BiaAgent.Actions;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Memory;
using AiCommerceApi.Features.BiaAgent.PendingActions;
using AiCommerceApi.Features.BiaAgent.Planning;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .CancelPendingAction;

public sealed class CancelPendingActionHandler
    : IBiaAgentActionHandler
{
    private readonly IBiaConversationMemory
        _conversationMemory;

    public CancelPendingActionHandler(
        IBiaConversationMemory conversationMemory)
    {
        _conversationMemory = conversationMemory;
    }

    public string Action =>
        BiaAgentActions.CancelPendingAction;

    public Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        if (!context.UserId.HasValue)
        {
            return Task.FromResult(
                CreateAuthenticationRequiredResponse(
                    context.ConversationId));
        }

        bool pendingActionFound =
            _conversationMemory.TryTakePendingAction(
                context.ConversationId,
                context.UserId.Value,
                out BiaPendingAction? pendingAction);

        if (!pendingActionFound ||
            pendingAction is null)
        {
            return Task.FromResult(
                CreatePendingActionNotFoundResponse(
                    context.ConversationId));
        }

        return Task.FromResult(
            new BiaChatResponseDto
            {
                ConversationId =
                    context.ConversationId,

                Action =
                    BiaAgentActions
                        .CancelPendingAction,

                Message =
                    $"{pendingAction.ProductName} için " +
                    "sepete ekleme işlemi iptal edildi.",

                RequiresAuthentication =
                    false,

                RequiresConfirmation =
                    false
            });
    }

    private static BiaChatResponseDto
        CreateAuthenticationRequiredResponse(
            Guid conversationId)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.CancelPendingAction,

            Message =
                "Sepet işlemini iptal edebilmem için " +
                "önce giriş yapmalısın.",

            RequiresAuthentication =
                true,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreatePendingActionNotFoundResponse(
            Guid conversationId)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions.CancelPendingAction,

            Message =
                "İptal edilebilecek, onay bekleyen " +
                "bir sepet işlemi bulunamadı.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false
        };
    }
}