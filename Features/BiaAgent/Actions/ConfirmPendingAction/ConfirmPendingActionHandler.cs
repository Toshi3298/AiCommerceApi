using AiCommerceApi.Features.BiaAgent.Actions;
using AiCommerceApi.Features.BiaAgent.Chat;
using AiCommerceApi.Features.BiaAgent.Memory;
using AiCommerceApi.Features.BiaAgent.PendingActions;
using AiCommerceApi.Features.BiaAgent.Planning;
using AiCommerceApi.Features.BiaAgent.Tools;

namespace AiCommerceApi.Features.BiaAgent.Actions
    .ConfirmPendingAction;

public sealed class ConfirmPendingActionHandler
    : IBiaAgentActionHandler
{
    private readonly IBiaConversationMemory
        _conversationMemory;

    private readonly IBiaCartTool
        _cartTool;

    public ConfirmPendingActionHandler(
        IBiaConversationMemory conversationMemory,
        IBiaCartTool cartTool)
    {
        _conversationMemory = conversationMemory;
        _cartTool = cartTool;
    }

    public string Action =>
        BiaAgentActions.ConfirmPendingAction;

    public async Task<BiaChatResponseDto> HandleAsync(
        BiaActionContext context,
        CancellationToken cancellationToken)
    {
        if (!context.UserId.HasValue)
        {
            return CreateAuthenticationRequiredResponse(
                context.ConversationId);
        }

        bool pendingActionFound =
            _conversationMemory.TryTakePendingAction(
                context.ConversationId,
                context.UserId.Value,
                out BiaPendingAction? pendingAction);

        if (!pendingActionFound ||
            pendingAction is null)
        {
            return CreatePendingActionNotFoundResponse(
                context.ConversationId);
        }

        var result =
            await _cartTool.AddItemAsync(
                pendingAction.UserId,
                pendingAction.ProductId,
                pendingAction.Quantity,
                cancellationToken);

        if (!result.Success)
        {
            return CreateCartFailureResponse(
                context.ConversationId,
                result.Error);
        }

        return new BiaChatResponseDto
        {
            ConversationId =
                context.ConversationId,

            Action =
                BiaAgentActions
                    .ConfirmPendingAction,

            Message =
                $"{pendingAction.ProductName} " +
                "sepetine başarıyla eklendi.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false,

            CartItemId =
                result.CartItemId,

            CartQuantity =
                result.Quantity
        };
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
                BiaAgentActions
                    .ConfirmPendingAction,

            Message =
                "Bu işlemi onaylamak için " +
                "giriş yapmalısın.",

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
                BiaAgentActions
                    .ConfirmPendingAction,

            Message =
                "Onay bekleyen bir işlem bulunamadı " +
                "veya işlemin süresi doldu.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false
        };
    }

    private static BiaChatResponseDto
        CreateCartFailureResponse(
            Guid conversationId,
            string? error)
    {
        return new BiaChatResponseDto
        {
            ConversationId =
                conversationId,

            Action =
                BiaAgentActions
                    .ConfirmPendingAction,

            Message =
                error ??
                "Ürün sepete eklenemedi.",

            RequiresAuthentication =
                false,

            RequiresConfirmation =
                false
        };
    }
}