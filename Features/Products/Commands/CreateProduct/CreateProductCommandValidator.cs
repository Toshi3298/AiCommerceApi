using FluentValidation;

namespace AiCommerceApi.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Ürün adı boş bırakılamaz.")
            .MaximumLength(150)
            .WithMessage(
                "Ürün adı en fazla 150 karakter olabilir.");

        RuleFor(command => command.Description)
            .MaximumLength(1000)
            .WithMessage(
                "Ürün açıklaması en fazla 1000 karakter olabilir.");

        RuleFor(command => command.Brand)
            .NotEmpty()
            .WithMessage("Marka boş bırakılamaz.")
            .MaximumLength(100)
            .WithMessage(
                "Marka en fazla 100 karakter olabilir.");

        RuleFor(command => command.Price)
            .GreaterThan(0)
            .WithMessage(
                "Ürün fiyatı sıfırdan büyük olmalıdır.");

        RuleFor(command => command.Stock)
            .GreaterThanOrEqualTo(0)
            .WithMessage(
                "Ürün stoğu negatif olamaz.");

        RuleFor(command => command.CategoryId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir kategori seçilmelidir.");
    }
}