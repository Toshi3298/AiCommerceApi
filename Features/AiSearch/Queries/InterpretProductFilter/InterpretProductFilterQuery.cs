using AiCommerceApi.Dtos.Ai;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .InterpretProductFilter;

public sealed record InterpretProductFilterQuery(
    string Prompt
) : IRequest<AiProductSearchFilterDto>;