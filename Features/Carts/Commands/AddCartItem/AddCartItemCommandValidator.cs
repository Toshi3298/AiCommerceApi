using FluentValidation;

namespace AiCommerceApi.Features.Carts.Commands.AddCartItem;

public class AddCartItemCommandValidator
    : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kullanıcı bilgisi bulunamadı.");

        RuleFor(command => command.ProductId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir ürün seçilmelidir.");

        RuleFor(command => command.Quantity)
            .GreaterThan(0)
            .WithMessage(
                "Ürün miktarı sıfırdan büyük olmalıdır.");
    }
}