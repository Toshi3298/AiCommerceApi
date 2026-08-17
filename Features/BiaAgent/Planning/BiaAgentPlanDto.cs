namespace AiCommerceApi.Features.BiaAgent.Planning;

public sealed class BiaAgentPlanDto
{
    public string Action { get; init; } =
        BiaAgentActions.Unsupported;

    public int? ProductId { get; init; }

    public string? ProductName { get; init; }
}