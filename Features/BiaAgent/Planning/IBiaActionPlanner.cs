namespace AiCommerceApi.Features.BiaAgent.Planning;

public interface IBiaActionPlanner
{
    Task<BiaAgentPlanDto> PlanAsync(
        string message,
        CancellationToken cancellationToken);
}