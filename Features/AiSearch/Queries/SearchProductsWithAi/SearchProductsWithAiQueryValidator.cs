using FluentValidation;

namespace AiCommerceApi.Features.AiSearch.Queries
    .SearchProductsWithAi;

public sealed class SearchProductsWithAiQueryValidator
    : AbstractValidator<SearchProductsWithAiQuery>
{
    public SearchProductsWithAiQueryValidator()
    {
        RuleFor(query => query.Prompt)
            .NotEmpty()
            .WithMessage("Arama isteği boş bırakılamaz.")
            .MinimumLength(3)
            .WithMessage("Arama isteği en az 3 karakter olmalıdır.")
            .MaximumLength(500)
            .WithMessage("Arama isteği en fazla 500 karakter olabilir.");
    }
}