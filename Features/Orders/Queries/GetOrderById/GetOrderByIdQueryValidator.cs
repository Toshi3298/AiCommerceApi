using FluentValidation;

namespace AiCommerceApi.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryValidator
    : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(query => query.UserId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir kullanıcı ID değeri bulunamadı.");

        RuleFor(query => query.OrderId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir sipariş ID değeri girilmelidir.");
    }
}