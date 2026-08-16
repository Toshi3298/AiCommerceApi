using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Services.Ai.Filters;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .InterpretProductFilter;

public sealed class InterpretProductFilterQueryHandler
    : IRequestHandler<
        InterpretProductFilterQuery,
        AiProductSearchFilterDto>
{
    private readonly IAiProductFilterInterpreter
        _filterInterpreter;

    public InterpretProductFilterQueryHandler(
        IAiProductFilterInterpreter filterInterpreter)
    {
        _filterInterpreter = filterInterpreter;
    }

    public async Task<AiProductSearchFilterDto> Handle(
        InterpretProductFilterQuery request,
        CancellationToken cancellationToken)
    {
        return await _filterInterpreter.InterpretAsync(
            request.Prompt,
            cancellationToken);
    }
}