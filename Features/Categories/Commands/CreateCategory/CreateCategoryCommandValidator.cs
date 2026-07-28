using FluentValidation;

namespace AiCommerceApi.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator
    : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
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