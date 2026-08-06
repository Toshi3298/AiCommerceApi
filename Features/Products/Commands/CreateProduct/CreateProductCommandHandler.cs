using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        bool categoryExists =
            await _context.Categories.AnyAsync(
                category =>
                    category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            return Failure("Kategori bulunamadı.");
        }

        var product = new Product
        {
            Name = request.Name.Trim(),

            Description =
                request.Description?.Trim()
                ?? string.Empty,

            Brand = request.Brand.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,

            ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl)
                ? null
                : request.ImageUrl.Trim(),

            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new CreateProductResult
        {
            Success = true,
            ProductId = product.Id
        };
    }

    private static CreateProductResult Failure(
        string error)
    {
        return new CreateProductResult
        {
            Success = false,
            Error = error
        };
    }
}