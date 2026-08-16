using FluentValidation;

namespace AiCommerceApi.Dtos.Ai;

public sealed class AiProductSearchFilterDtoValidator
    : AbstractValidator<AiProductSearchFilterDto>
{
    private static readonly string[] AllowedSortFields =
    [
        "name",
        "price",
        "stock",
        "createdat"
    ];

    private static readonly string[] AllowedDirections =
    [
        "asc",
        "desc"
    ];

    public AiProductSearchFilterDtoValidator()
    {
        RuleFor(filter => filter.Search)
            .MaximumLength(150);

        RuleFor(filter => filter.Brand)
            .MaximumLength(100);

        RuleFor(filter => filter.CategoryName)
            .MaximumLength(150);

        RuleFor(filter => filter.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(filter => filter.MinPrice.HasValue);

        RuleFor(filter => filter.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(filter => filter.MaxPrice.HasValue);

        RuleFor(filter => filter)
            .Must(filter =>
                !filter.MinPrice.HasValue ||
                !filter.MaxPrice.HasValue ||
                filter.MinPrice <= filter.MaxPrice)
            .WithMessage(
                "Minimum fiyat maksimum fiyattan büyük olamaz.");

        RuleFor(filter => filter.SortBy)
            .Must(value =>
                AllowedSortFields.Contains(
                    value.ToLowerInvariant()))
            .WithMessage(
                "Geçersiz sıralama alanı.");

        RuleFor(filter => filter.SortDirection)
            .Must(value =>
                AllowedDirections.Contains(
                    value.ToLowerInvariant()))
            .WithMessage(
                "Geçersiz sıralama yönü.");

        RuleFor(filter => filter.Limit)
            .InclusiveBetween(1, 50)
            .WithMessage(
                "Ürün limiti 1 ile 50 arasında olmalıdır.");
    }
}