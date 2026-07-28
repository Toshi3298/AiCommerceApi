using FluentValidation;

namespace AiCommerceApi.Features.Carts.Commands.UpdateCartItem;

public class UpdateCartItemCommandValidator
    : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kullanıcı bilgisi bulunamadı.");

        RuleFor(command => command.CartItemId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir sepet ürünü seçilmelidir.");

        RuleFor(command => command.Quantity)
            .GreaterThan(0)
            .WithMessage(
                "Ürün miktarı sıfırdan büyük olmalıdır.");
    }
}