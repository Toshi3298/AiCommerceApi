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

        // İlk SQL üretimi
        string generatedSql =
            await _sqlGenerator.GenerateSqlAsync(
                normalizedPrompt,
                cancellationToken);

        // İlk güvenlik kontrolü
        var validationResult =
            _sqlSecurityService.Validate(generatedSql);

        // SQL güvenli değilse modele yalnızca bir kez
        // düzelttiriyoruz.
        if (!validationResult.IsValid)
        {
            string correctionPrompt = $"""
                Kullanıcı isteği:
                {normalizedPrompt}

                Önceki üretilen sorgu güvenlik
                kontrolünden geçmedi.

                Güvenlik hatası:
                {validationResult.Error}

                Sorguyu aşağıdaki zorunlu kurallara
                göre yeniden üret:

                - SELECT TOP (50) ile başlamalıdır.
                - Her sorguda p.IsActive = 1 bulunmalıdır.
                - Products tablosuna p takma adı verilmelidir.
                - Categories tablosuna c takma adı verilmelidir.
                - İki tablo INNER JOIN yapılmalıdır.
                - Yalnızca Products ve Categories kullanılmalıdır.
                - Yalnızca tek bir SELECT sorgusu üretilmelidir.
                - Açıklama veya markdown kullanılmamalıdır.
                - Sadece çalıştırılabilir T-SQL döndürülmelidir.
                """;

            generatedSql =
                await _sqlGenerator.GenerateSqlAsync(
                    correctionPrompt,
                    cancellationToken);

            // Düzeltilen sorgu da tekrar güvenlik
            // kontrolünden geçiriliyor.
            validationResult =
                _sqlSecurityService.Validate(generatedSql);
        }

        // İkinci sorgu da güvenli değilse veritabanında
        // kesinlikle çalıştırılmıyor.
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException(
                validationResult.Error
                ?? "Üretilen SQL sorgusu güvenli değil.");
        }

        // Yalnızca güvenlik kontrolünden geçen sorgu
        // salt-okunur bağlantıyla çalıştırılıyor.
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