using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace AiCommerceApi.Services.Ai;

public sealed class OllamaAiSqlGenerator : IAiSqlGenerator
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _systemPromptPath;

    public OllamaAiSqlGenerator(
        HttpClient httpClient,
        IConfiguration configuration,
        IHostEnvironment environment)
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

        _systemPromptPath = Path.Combine(
            environment.ContentRootPath,
            "Prompts",
            "AiProductSearchSqlPrompt.md");

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(90);
    }

    public async Task<string> GenerateSqlAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(_systemPromptPath))
        {
            throw new FileNotFoundException(
                "AI SQL sistem prompt dosyası bulunamadı.",
                _systemPromptPath);
        }

        string systemPrompt =
            await File.ReadAllTextAsync(
                _systemPromptPath,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            throw new InvalidOperationException(
                "AI SQL sistem prompt dosyası boş olamaz.");
        }

        var request = new OllamaGenerateRequest
        {
            Model = _model,
            System = systemPrompt,
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

    private static string CleanSql(string response)
    {
        string sql = response.Trim();

        if (sql.StartsWith(
                "```sql",
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