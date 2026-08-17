using FluentValidation;

namespace AiCommerceApi.Features.BiaAgent.Chat;

public sealed class BiaChatQueryValidator
    : AbstractValidator<BiaChatQuery>
{
    public BiaChatQueryValidator()
    {
        RuleFor(query => query.Message)
            .NotEmpty()
            .WithMessage(
                "Bia mesajı boş bırakılamaz.")
            .MinimumLength(3)
            .WithMessage(
                "Bia mesajı en az 3 karakter olmalıdır.")
            .MaximumLength(500)
            .WithMessage(
                "Bia mesajı en fazla 500 karakter olabilir.");
    }
}