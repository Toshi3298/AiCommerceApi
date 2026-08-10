using FluentValidation;

namespace AiCommerceApi.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator
    : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kategori ID değeri gönderilmelidir.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage(
                "Kategori adı boş bırakılamaz.")
            .MaximumLength(150)
            .WithMessage(
                "Kategori adı en fazla 150 karakter olabilir.");

        RuleFor(command => command.Description)
            .MaximumLength(500)
            .WithMessage(
                "Kategori açıklaması en fazla 500 karakter olabilir.");
    }
}