using AiCommerceApi.Dtos.Ai;

namespace AiCommerceApi.Services.Ai.Filters;

public interface IAiProductFilterInterpreter
{
    Task<AiProductSearchFilterDto> InterpretAsync(
        string prompt,
        CancellationToken cancellationToken);
}