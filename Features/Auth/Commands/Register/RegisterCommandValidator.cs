using FluentValidation;

namespace AiCommerceApi.Features.Auth.Commands.Register;

public class RegisterCommandValidator
    : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(command => command.FirstName)
            .NotEmpty()
            .WithMessage("Ad boş bırakılamaz.")
            .MaximumLength(100)
            .WithMessage(
                "Ad en fazla 100 karakter olabilir.");

        RuleFor(command => command.LastName)
            .NotEmpty()
            .WithMessage("Soyad boş bırakılamaz.")
            .MaximumLength(100)
            .WithMessage(
                "Soyad en fazla 100 karakter olabilir.");

        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email boş bırakılamaz.")
            .EmailAddress()
            .WithMessage("Geçerli bir email adresi girilmelidir.")
            .MaximumLength(250)
            .WithMessage(
                "Email en fazla 250 karakter olabilir.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Şifre boş bırakılamaz.")
            .MinimumLength(8)
            .WithMessage(
                "Şifre en az 8 karakter olmalıdır.")
            .MaximumLength(100)
            .WithMessage(
                "Şifre en fazla 100 karakter olabilir.");
    }
}