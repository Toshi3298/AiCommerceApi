using FluentValidation;

namespace AiCommerceApi.Features.Admin.Orders.Queries
    .GetAdminOrderById;

public class GetAdminOrderByIdQueryValidator
    : AbstractValidator<GetAdminOrderByIdQuery>
{
    public GetAdminOrderByIdQueryValidator()
    {
        RuleFor(query => query.OrderId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir sipariş ID değeri gönderilmelidir.");
    }
}