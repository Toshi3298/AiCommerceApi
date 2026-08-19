namespace AiCommerceApi.Features.BiaAgent.PendingActions;

public sealed class BiaPendingAction
{
    public string Action { get; init; } =
        string.Empty;

    public int UserId { get; init; }

    public int ProductId { get; init; }

    public string ProductName { get; init; } =
        string.Empty;

    public int Quantity { get; init; }

    public DateTime CreatedAt { get; init; } =
        DateTime.UtcNow;
}