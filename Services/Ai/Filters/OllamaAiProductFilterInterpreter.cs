using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using AiCommerceApi.Dtos.Ai;

namespace AiCommerceApi.Services.Ai.Filters;

public sealed class OllamaAiProductFilterInterpreter
    : IAiProductFilterInterpreter
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _systemPromptPath;

    private static readonly JsonSerializerOptions
        JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

    public OllamaAiProductFilterInterpreter(
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
            "AiProductFilterPrompt.md");

        _httpClient.BaseAddress = new Uri(baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(90);
    }

    public async Task<AiProductSearchFilterDto>
        InterpretAsync(
            string prompt,
            CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException(
                "Ürün arama isteği boş olamaz.",
                nameof(prompt));
        }

        if (!File.Exists(_systemPromptPath))
        {
            throw new FileNotFoundException(
                "AI ürün filtre prompt dosyası bulunamadı.",
                _systemPromptPath);
        }

        string systemPrompt =
            await File.ReadAllTextAsync(
                _systemPromptPath,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            throw new InvalidOperationException(
                "AI ürün filtre prompt dosyası boş olamaz.");
        }

        var request = new OllamaGenerateRequest
        {
            Model = _model,
            System = systemPrompt,
            Prompt = prompt.Trim(),
            Format = "json",
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
                "Ollama geçerli bir ürün filtresi üretmedi.");
        }

        try
        {
            var filter =
                JsonSerializer
                    .Deserialize<AiProductSearchFilterDto>(
                        result.Response,
                        JsonOptions);

            return filter
                ?? throw new InvalidOperationException(
                    "Ollama ürün filtresi boş döndü.");
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "Ollama ürün filtresini geçerli JSON biçiminde döndürmedi.",
                exception);
        }
    }

    private sealed class OllamaGenerateRequest
    {
        public string Model { get; init; } =
            string.Empty;

        public string System { get; init; } =
            string.Empty;

        public string Prompt { get; init; } =
            string.Empty;

        public string Format { get; init; } =
            "json";

        public bool Stream { get; init; }

        public OllamaOptions Options { get; init; } =
            new();
    }

    private sealed class OllamaOptions
    {
        public double Temperature { get; init; }
    }

    private sealed class OllamaGenerateResponse
    {
        [JsonPropertyName("response")]
        public string Response { get; init; } =
            string.Empty;
    }
}