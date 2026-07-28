using FluentValidation;

namespace AiCommerceApi.Features.Auth.Commands.Login;

public class LoginCommandValidator
    : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .WithMessage("Email boş bırakılamaz.")
            .EmailAddress()
            .WithMessage(
                "Geçerli bir email adresi girilmelidir.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .WithMessage("Şifre boş bırakılamaz.");
    }
}