using AiCommerceApi.Dtos.Ai;
using MediatR;

namespace AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithAi;

public sealed record SearchProductsWithAiQuery(
    string Prompt)
    : IRequest<AiSearchResponseDto>;