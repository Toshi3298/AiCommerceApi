using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler
    : IRequestHandler<
        UpdateCategoryCommand,
        UpdateCategoryResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateCategoryCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCategoryResult> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(
                category => category.Id == request.Id,
                cancellationToken);

        if (category is null)
        {
            return new UpdateCategoryResult
            {
                Success = false,
                NotFound = true,
                Error = "Kategori bulunamadı."
            };
        }

        string categoryName = request.Name.Trim();

        bool categoryNameExists =
            await _context.Categories.AnyAsync(
                otherCategory =>
                    otherCategory.Id != request.Id &&
                    otherCategory.Name == categoryName,
                cancellationToken);

        if (categoryNameExists)
        {
            return new UpdateCategoryResult
            {
                Success = false,
                Error = "Bu kategori adı zaten kullanılıyor."
            };
        }

        category.Name = categoryName;

        category.Description =
            request.Description?.Trim()
            ?? string.Empty;

        await _context.SaveChangesAsync(
            cancellationToken);

        return new UpdateCategoryResult
        {
            Success = true
        };
    }
}