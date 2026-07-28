using FluentValidation;

namespace AiCommerceApi.Features.Carts.Commands.RemoveCartItem;

public class RemoveCartItemCommandValidator
    : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kullanıcı bilgisi bulunamadı.");

        RuleFor(command => command.CartItemId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir sepet ürünü seçilmelidir.");
    }
}