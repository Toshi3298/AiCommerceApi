using FluentValidation;

namespace AiCommerceApi.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandValidator
    : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir ürün ID değeri gönderilmelidir.");
    }
}