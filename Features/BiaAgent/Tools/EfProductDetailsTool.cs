using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.BiaAgent.Tools;

public sealed class EfProductDetailsTool
    : IAiProductDetailsTool
{
    private readonly ApplicationDbContext _context;

    public EfProductDetailsTool(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> GetByIdAsync(
        int productId,
        CancellationToken cancellationToken)
    {
        if (productId <= 0)
        {
            return null;
        }

        IQueryable<Product> query =
            _context.Products
                .AsNoTracking()
                .Where(product =>
                    product.Id == productId &&
                    product.IsActive);

        return await ProjectToDto(query)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ProductDto?> FindByNameAsync(
        string productName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            return null;
        }

        string normalizedName =
            productName.Trim();

        IQueryable<Product> query =
            _context.Products
                .AsNoTracking()
                .Where(product =>
                    product.IsActive &&
                    product.Name.Contains(
                        normalizedName))
                .OrderBy(product =>
                    product.Name == normalizedName
                        ? 0
                        : 1)
                .ThenBy(product => product.Name);

        return await ProjectToDto(query)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static IQueryable<ProductDto> ProjectToDto(
        IQueryable<Product> query)
    {
        return query.Select(product =>
            new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price,
                Stock = product.Stock,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl,
                CategoryName =
                    product.Category.Name
            });
    }
}