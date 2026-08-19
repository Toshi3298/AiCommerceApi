using AiCommerceApi.Features.BiaAgent.Planning;

namespace AiCommerceApi.Features.BiaAgent.Actions;

public sealed record BiaActionContext(
    string Message,
    Guid ConversationId,
    int? UserId,
    BiaAgentPlanDto Plan);