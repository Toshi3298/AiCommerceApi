namespace AiCommerceApi.Services.Ai;

public sealed class MockAiSqlGenerator : IAiSqlGenerator
{
    public Task<string> GenerateSqlAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        string generatedSql = """
            SELECT TOP (50)
                p.Id,
                p.Name,
                p.Description,
                p.Brand,
                p.Price,
                p.Stock,
                p.IsActive,
                p.CreatedAt,
                p.CategoryId,
                c.Name AS CategoryName
            FROM Products AS p
            INNER JOIN Categories AS c
                ON c.Id = p.CategoryId
            WHERE p.IsActive = 1
            ORDER BY p.Name;
            """;
        return Task.FromResult(generatedSql);
    }
}