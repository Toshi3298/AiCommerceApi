using FluentValidation;

namespace AiCommerceApi.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(command => command.UserId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kullanıcı bilgisi bulunamadı.");

        RuleFor(command => command.ShippingAddress)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                "Teslimat adresi boş bırakılamaz.")
            .MinimumLength(10)
            .WithMessage(
                "Teslimat adresi en az 10 karakter olmalıdır.")
            .MaximumLength(500)
            .WithMessage(
                "Teslimat adresi en fazla 500 karakter olabilir.");
    }
}