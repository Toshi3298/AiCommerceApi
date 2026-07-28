using FluentValidation;

namespace AiCommerceApi.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryValidator
    : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(query => query.Id)
            .GreaterThan(0)
            .WithMessage("Geçerli bir ürün ID değeri girilmelidir.");
    }
}