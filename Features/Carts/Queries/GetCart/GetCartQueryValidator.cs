using FluentValidation;

namespace AiCommerceApi.Features.Carts.Queries.GetCart;

public sealed class GetCartQueryValidator
    : AbstractValidator<GetCartQuery>
{
    public GetCartQueryValidator()
    {
        RuleFor(query => query.UserId)
            .GreaterThan(0)
            .WithMessage("Geçerli bir kullanıcı ID değeri bulunamadı.");
    }
}