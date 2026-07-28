using FluentValidation;

namespace AiCommerceApi.Features.Carts.Commands.ClearCart;

public class ClearCartCommandValidator
    : AbstractValidator<ClearCartCommand>
{
    public ClearCartCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kullanıcı bilgisi bulunamadı.");
    }
}