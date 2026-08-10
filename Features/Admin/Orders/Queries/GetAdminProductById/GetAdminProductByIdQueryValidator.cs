using FluentValidation;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProductById;

public sealed class GetAdminProductByIdQueryValidator
    : AbstractValidator<GetAdminProductByIdQuery>
{
    public GetAdminProductByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir ürün ID değeri gönderilmelidir.");
    }
}