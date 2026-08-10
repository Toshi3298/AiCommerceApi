using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler
    : IRequestHandler<
        DeleteCategoryCommand,
        DeleteCategoryResult>
{
    private readonly ApplicationDbContext _context;

    public DeleteCategoryCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteCategoryResult> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == request.Id,
                cancellationToken);

        if (category is null)
        {
            return new DeleteCategoryResult
            {
                Success = false,
                NotFound = true,
                Error = "Kategori bulunamadı."
            };
        }

        bool hasProducts =
            await _context.Products.AnyAsync(
                product =>
                    product.CategoryId == request.Id,
                cancellationToken);

        if (hasProducts)
        {
            return new DeleteCategoryResult
            {
                Success = false,
                InUse = true,
                Error =
                    "Bu kategoriye bağlı ürünler bulunduğu için kategori silinemez."
            };
        }

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new DeleteCategoryResult
        {
            Success = true
        };
    }
}