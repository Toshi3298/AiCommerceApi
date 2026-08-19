using AiCommerceApi.Features.BiaAgent.Memory;
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

    private readonly IBiaConversationMemory
        _conversationMemory;

    public BiaChatQueryHandler(
        IBiaAgentOrchestrator orchestrator,
        IBiaConversationMemory conversationMemory)
    {
        _orchestrator = orchestrator;
        _conversationMemory = conversationMemory;
    }

    public async Task<BiaChatResponseDto> Handle(
        BiaChatQuery request,
        CancellationToken cancellationToken)
    {
        Guid conversationId =
            request.ConversationId.HasValue &&
            request.ConversationId.Value != Guid.Empty
                ? request.ConversationId.Value
                : Guid.NewGuid();

        BiaChatResponseDto agentResponse =
            await _orchestrator.ChatAsync(
                request.Message.Trim(),
                conversationId,
                request.UserId,
                cancellationToken);

        if (agentResponse.Products.Count > 0)
        {
            _conversationMemory.SaveProductIds(
                conversationId,
                agentResponse.Products.Select(
                    product => product.Id));
        }

        return new BiaChatResponseDto
        {
            ConversationId = conversationId,
            Action = agentResponse.Action,
            Message = agentResponse.Message,
            Products = agentResponse.Products,
            Product = agentResponse.Product,

            RequiresAuthentication =
                agentResponse.RequiresAuthentication,

            RequiresConfirmation =
                agentResponse.RequiresConfirmation,

            CartItemId =
                agentResponse.CartItemId,

            CartQuantity =
                agentResponse.CartQuantity
        };
    }
}