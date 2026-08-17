using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiCommerceApi.Features.BiaAgent.Planning;

public sealed class OllamaBiaActionPlanner
    : IBiaActionPlanner
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private readonly string _promptPath;

    private static readonly JsonSerializerOptions
        JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

    public OllamaBiaActionPlanner(
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

        _promptPath = Path.Combine(
            environment.ContentRootPath,
            "Features",
            "BiaAgent",
            "Prompts",
            "BiaActionPlannerPrompt.md");

        _httpClient.BaseAddress =
            new Uri(baseUrl);

        _httpClient.Timeout =
            TimeSpan.FromSeconds(90);
    }

    public async Task<BiaAgentPlanDto> PlanAsync(
        string message,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(
                "Bia mesajı boş olamaz.",
                nameof(message));
        }

        if (!File.Exists(_promptPath))
        {
            throw new FileNotFoundException(
                "Bia action planner prompt dosyası " +
                "bulunamadı.",
                _promptPath);
        }

        string systemPrompt =
            await File.ReadAllTextAsync(
                _promptPath,
                cancellationToken);

        if (string.IsNullOrWhiteSpace(systemPrompt))
        {
            throw new InvalidOperationException(
                "Bia action planner prompt dosyası " +
                "boş olamaz.");
        }

        var request = new OllamaGenerateRequest
        {
            Model = _model,
            System = systemPrompt,
            Prompt = message.Trim(),
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
                .ReadFromJsonAsync<
                    OllamaGenerateResponse>(
                    cancellationToken:
                        cancellationToken);

        if (string.IsNullOrWhiteSpace(
                result?.Response))
        {
            throw new InvalidOperationException(
                "Ollama geçerli bir Bia planı " +
                "üretmedi.");
        }

        try
        {
            var plan =
                JsonSerializer
                    .Deserialize<BiaAgentPlanDto>(
                        result.Response,
                        JsonOptions);

            if (plan is null)
            {
                throw new InvalidOperationException(
                    "Ollama Bia planını boş döndürdü.");
            }

            return NormalizePlan(plan);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException(
                "Ollama Bia planını geçerli JSON " +
                "biçiminde döndürmedi.",
                exception);
        }
    }

    private static BiaAgentPlanDto NormalizePlan(
        BiaAgentPlanDto plan)
    {
        string action =
            plan.Action?
                .Trim()
                .ToLowerInvariant()
            ?? BiaAgentActions.Unsupported;

        action = action switch
        {
            BiaAgentActions.SearchProducts =>
                BiaAgentActions.SearchProducts,

            BiaAgentActions.GetProductDetails =>
                BiaAgentActions.GetProductDetails,

            BiaAgentActions.SearchThenGetDetails =>
                BiaAgentActions.SearchThenGetDetails,

            _ => BiaAgentActions.Unsupported
        };

        int? productId =
            plan.ProductId > 0
                ? plan.ProductId
                : null;

        string? productName =
            string.IsNullOrWhiteSpace(
                plan.ProductName)
                ? null
                : plan.ProductName.Trim();

        if (action ==
                BiaAgentActions.GetProductDetails &&
            productId is null &&
            productName is null)
        {
            action = BiaAgentActions.Unsupported;
        }

        return new BiaAgentPlanDto
        {
            Action = action,
            ProductId = productId,
            ProductName = productName
        };
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