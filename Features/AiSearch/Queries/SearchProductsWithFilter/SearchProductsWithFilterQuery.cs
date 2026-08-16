using AiCommerceApi.Dtos.Ai;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithFilter;

public sealed record SearchProductsWithFilterQuery(
    string Prompt
) : IRequest<AiFilterSearchResponseDto>;