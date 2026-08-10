using AiCommerceApi.Models;
using FluentValidation;

namespace AiCommerceApi.Features.Admin.Orders.Queries.GetAdminOrders;

public class GetAdminOrdersQueryValidator
    : AbstractValidator<GetAdminOrdersQuery>
{
    public GetAdminOrdersQueryValidator()
    {
        RuleFor(query => query.Search)
            .MaximumLength(150)
            .WithMessage(
                "Arama değeri en fazla 150 karakter olabilir.");

        RuleFor(query => query.Status)
            .Must(BeValidOrderStatus)
            .When(query =>
                !string.IsNullOrWhiteSpace(query.Status))
            .WithMessage(
                "Geçerli bir sipariş durumu gönderilmelidir.");

        RuleFor(query => query.PageNumber)
            .GreaterThan(0)
            .WithMessage(
                "Sayfa numarası sıfırdan büyük olmalıdır.");

        RuleFor(query => query.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Sayfa boyutu 1 ile 100 arasında olmalıdır.");
    }

    private static bool BeValidOrderStatus(
        string? status)
    {
        return Enum.TryParse<OrderStatus>(
            status,
            ignoreCase: true,
            out _);
    }
}