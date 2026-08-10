using FluentValidation;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProducts;

public class GetAdminProductsQueryValidator
    : AbstractValidator<GetAdminProductsQuery>
{
    private static readonly string[] AllowedSortFields =
    {
        "name",
        "price",
        "stock",
        "createdat"
    };

    public GetAdminProductsQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(150)
            .WithMessage(
                "Arama değeri en fazla 150 karakter olabilir.");

        RuleFor(query => query.Brand)
            .MaximumLength(100)
            .WithMessage(
                "Marka değeri en fazla 100 karakter olabilir.");

        RuleFor(query => query.CategoryId)
            .GreaterThan(0)
            .When(query => query.CategoryId.HasValue)
            .WithMessage(
                "Geçerli bir kategori ID değeri gönderilmelidir.");

        RuleFor(query => query.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MinPrice.HasValue)
            .WithMessage(
                "Minimum fiyat negatif olamaz.");

        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(query => query.MaxPrice.HasValue)
            .WithMessage(
                "Maksimum fiyat negatif olamaz.");

        RuleFor(query => query)
            .Must(query =>
                !query.MinPrice.HasValue ||
                !query.MaxPrice.HasValue ||
                query.MinPrice.Value <= query.MaxPrice.Value)
            .WithMessage(
                "Minimum fiyat maksimum fiyattan büyük olamaz.");

        RuleFor(query => query.SortBy)
            .Must(sortBy =>
                string.IsNullOrWhiteSpace(sortBy) ||
                AllowedSortFields.Contains(
                    sortBy.Trim().ToLowerInvariant()))
            .WithMessage(
                "Sıralama alanı name, price, stock veya createdAt olmalıdır.");

        RuleFor(query => query.SortDirection)
            .Must(sortDirection =>
                string.IsNullOrWhiteSpace(sortDirection) ||
                sortDirection.Equals(
                    "asc",
                    StringComparison.OrdinalIgnoreCase) ||
                sortDirection.Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase))
            .WithMessage(
                "Sıralama yönü asc veya desc olmalıdır.");

        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage(
                "Sayfa numarası sıfırdan büyük olmalıdır.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Sayfa boyutu 1 ile 100 arasında olmalıdır.");
    }
}