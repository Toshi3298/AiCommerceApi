using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Services.Ai;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithAi;

public sealed class SearchProductsWithAiQueryHandler
    : IRequestHandler<
        SearchProductsWithAiQuery,
        AiSearchResponseDto>
{
    private readonly IAiSqlGenerator _sqlGenerator;
    private readonly ISqlSecurityService _sqlSecurityService;
    private readonly ISqlQueryExecutor _sqlQueryExecutor;

    public SearchProductsWithAiQueryHandler(
        IAiSqlGenerator sqlGenerator,
        ISqlSecurityService sqlSecurityService,
        ISqlQueryExecutor sqlQueryExecutor)
    {
        _sqlGenerator = sqlGenerator;
        _sqlSecurityService = sqlSecurityService;
        _sqlQueryExecutor = sqlQueryExecutor;
    }

    public async Task<AiSearchResponseDto> Handle(
        SearchProductsWithAiQuery request,
        CancellationToken cancellationToken)
    {
        string normalizedPrompt =
            request.Prompt.Trim();

        string generatedSql =
            await _sqlGenerator.GenerateSqlAsync(
                normalizedPrompt,
                cancellationToken);

        var validationResult =
            _sqlSecurityService.Validate(generatedSql);

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(
                validationResult.Error
                ?? "Üretilen SQL sorgusu güvenli değil.");
        }

        var products =
            await _sqlQueryExecutor.ExecuteProductQueryAsync(
                generatedSql,
                cancellationToken);

        return new AiSearchResponseDto
        {
            Prompt = normalizedPrompt,
            GeneratedSql = generatedSql,
            Products = products
        };
    }
}