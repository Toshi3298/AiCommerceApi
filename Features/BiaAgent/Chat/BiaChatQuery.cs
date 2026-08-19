using MediatR;

namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed record BiaChatQuery(
    string Message,
    Guid? ConversationId,
    int? UserId
) : IRequest<BiaChatResponseDto>;