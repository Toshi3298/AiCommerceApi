using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiCommerceApi.Services.Ai;

public sealed class OllamaAiSqlGenerator
    : IAiSqlGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public OllamaAiSqlGenerator(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        string baseUrl =
            configuration["Ollama:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Ollama BaseUrl bulunamadı.");

        _model =
            configuration["Ollama:Model"]
            ?? throw new InvalidOperationException(
                "Ollama model bilgisi bulunamadı.");

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(90);
    }

    public async Task<string> GenerateSqlAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var request = new OllamaGenerateRequest
        {
            Model = _model,
            System = CreateSystemPrompt(),
            Prompt = prompt,
            Stream = false,
            Options = new OllamaOptions
            {
                Temperature = 0
            }
        };

        using var response =
            await _httpClient.PostAsJsonAsync(
                "api/generate",
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content
                .ReadFromJsonAsync<OllamaGenerateResponse>(
                    cancellationToken: cancellationToken);

        if (string.IsNullOrWhiteSpace(result?.Response))
        {
            throw new InvalidOperationException(
                "Ollama geçerli bir SQL sorgusu üretmedi.");
        }

        return CleanSql(result.Response);
    }

    private static string CreateSystemPrompt()
    {
        return """
            Sen yalnızca Microsoft SQL Server için T-SQL
            sorgusu üreten bir servissin.

            Kullanılabilecek tablolar:

            Products:
            Id int,
            Name nvarchar,
            Description nvarchar,
            Brand nvarchar,
            Price decimal,
            Stock int,
            IsActive bit,
            CreatedAt datetime2,
            CategoryId int

            Categories:
            Id int,
            Name nvarchar,
            Description nvarchar

            İlişki:
            Products.CategoryId = Categories.Id

            Zorunlu kurallar:
            - Yalnızca tek bir SELECT sorgusu üret.
            - Sorgu SELECT TOP (50) ile başlamalıdır.
            - Products tablosuna p takma adı ver.
            - Categories tablosuna c takma adı ver.
            - Products ile Categories tablolarını INNER JOIN yap.
            - Yalnızca Products ve Categories tablolarını kullan.
            - INSERT, UPDATE, DELETE, DROP, ALTER, EXEC,
              MERGE ve benzeri komutları asla kullanma.
            - Aktif ürünler için p.IsActive = 1 koşulunu kullan.
            - Kullanıcı stokta ürün isterse p.Stock > 0 kullan.
            - Para değerlerini yalnızca sayısal değer olarak kullan.
            - Aşağıdaki kolonları ve aynı takma adları döndür:

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

            - Açıklama yazma.
            - Markdown kod bloğu kullanma.
            - Sadece çalıştırılabilir T-SQL metnini döndür.
            """;
    }

    private static string CleanSql(string response)
    {
        string sql = response.Trim();

        if (sql.StartsWith("```sql",
                StringComparison.OrdinalIgnoreCase))
        {
            sql = sql[6..];
        }
        else if (sql.StartsWith("```"))
        {
            sql = sql[3..];
        }

        if (sql.EndsWith("```"))
        {
            sql = sql[..^3];
        }

        return sql.Trim();
    }

    private sealed class OllamaGenerateRequest
    {
        public string Model { get; init; } = string.Empty;

        public string System { get; init; } = string.Empty;

        public string Prompt { get; init; } = string.Empty;

        public bool Stream { get; init; }

        public OllamaOptions Options { get; init; } = new();
    }

    private sealed class OllamaOptions
    {
        public double Temperature { get; init; }
    }

    private sealed class OllamaGenerateResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; init; } = string.Empty;
    }
}