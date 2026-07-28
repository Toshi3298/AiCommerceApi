using FluentValidation;

namespace AiCommerceApi.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryValidator
    : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields =
    {
        "name",
        "price",
        "stock",
        "createdat"
    };

    private static readonly string[] AllowedSortDirections =
    {
        "asc",
        "desc"
    };

    public GetProductsQueryValidator()
    {
        RuleFor(query => query.CategoryId)
            .GreaterThan(0)
            .When(query => query.CategoryId.HasValue)
            .WithMessage("Geçerli bir kategori seçilmelidir.");

        RuleFor(query => query.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MinPrice.HasValue)
            .WithMessage("Minimum fiyat negatif olamaz.");

        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MaxPrice.HasValue)
            .WithMessage("Maksimum fiyat negatif olamaz.");

        RuleFor(query => query)
            .Must(query =>
                !query.MinPrice.HasValue ||
                !query.MaxPrice.HasValue ||
                query.MinPrice.Value <= query.MaxPrice.Value)
            .WithMessage(
                "Minimum fiyat maksimum fiyattan büyük olamaz.");

        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage("Sayfa numarası sıfırdan büyük olmalıdır.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        RuleFor(query => query.SortBy)
            .Must(BeValidSortField)
            .WithMessage(
                "Sıralama alanı name, price, stock veya createdAt olmalıdır.");

        RuleFor(query => query.SortDirection)
            .Must(BeValidSortDirection)
            .WithMessage(
                "Sıralama yönü asc veya desc olmalıdır.");
    }

    private static bool BeValidSortField(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return true;
        }

        return AllowedSortFields.Contains(
            sortBy.Trim().ToLowerInvariant());
    }

    private static bool BeValidSortDirection(
        string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            return true;
        }

        return AllowedSortDirections.Contains(
            sortDirection.Trim().ToLowerInvariant());
    }
}