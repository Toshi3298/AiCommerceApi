using FluentValidation;

namespace AiCommerceApi.Features.Orders.Queries.GetOrders;

public sealed class GetOrdersQueryValidator
    : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(query => query.UserId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir kullanıcı ID değeri bulunamadı.");
    }
}