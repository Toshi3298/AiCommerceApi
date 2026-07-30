namespace AiCommerceApi.Services.Ai;

public interface IAiSqlGenerator
{
    Task<string> GenerateSqlAsync(
        string prompt,
        CancellationToken cancellationToken);
}