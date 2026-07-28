using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<
        CreateCategoryCommand,
        CreateCategoryResult>
{
    private readonly ApplicationDbContext _context;

    public CreateCategoryCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateCategoryResult> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        string categoryName = request.Name.Trim();

        bool categoryExists =
            await _context.Categories.AnyAsync(
                category =>
                    category.Name == categoryName,
                cancellationToken);

        if (categoryExists)
        {
            return new CreateCategoryResult
            {
                Success = false,
                Error = "Bu kategori zaten bulunuyor."
            };
        }

        var category = new Category
        {
            Name = categoryName,

            Description =
                request.Description?.Trim()
                ?? string.Empty
        };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync(
            cancellationToken);

        return new CreateCategoryResult
        {
            Success = true,
            CategoryId = category.Id
        };
    }
}