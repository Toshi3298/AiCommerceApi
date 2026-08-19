namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed class BiaChatRequestDto
{
    public string Message { get; init; } =
        string.Empty;

    public Guid? ConversationId { get; init; }
}