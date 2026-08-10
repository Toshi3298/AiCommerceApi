using FluentValidation;

namespace AiCommerceApi.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator
    : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kategori ID değeri gönderilmelidir.");
    }
}