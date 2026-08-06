using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<
        UpdateProductCommand,
        UpdateProductResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateProductCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProductResult> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            return new UpdateProductResult
            {
                Success = false,
                NotFound = true,
                Error = "Ürün bulunamadı."
            };
        }

        bool categoryExists =
            await _context.Categories.AnyAsync(
                category =>
                    category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            return Failure("Kategori bulunamadı.");
        }

        product.Name = request.Name.Trim();

        product.Description =
            request.Description?.Trim()
            ?? string.Empty;

        product.Brand = request.Brand.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;
        product.ImageUrl =
            string.IsNullOrWhiteSpace(request.ImageUrl)
                ? null
                : request.ImageUrl.Trim();

        await _context.SaveChangesAsync(
            cancellationToken);

        return new UpdateProductResult
        {
            Success = true
        };
    }

    private static UpdateProductResult Failure(
        string error)
    {
        return new UpdateProductResult
        {
            Success = false,
            Error = error
        };
    }
}