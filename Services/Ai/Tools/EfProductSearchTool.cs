using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Ai;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Services.Ai.Tools;
public sealed class EfProductSearchTool
    : IAiProductSearchTool
{
    private readonly ApplicationDbContext _context;
    private readonly IValidator<AiProductSearchFilterDto>
        _validator;

    public EfProductSearchTool(
        ApplicationDbContext context,
        IValidator<AiProductSearchFilterDto> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<List<ProductDto>> SearchAsync(
        AiProductSearchFilterDto filter,
        CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(
            filter,
            cancellationToken);

        IQueryable<Product> query =
            _context.Products
                .AsNoTracking()
                .Where(product => product.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            string search = filter.Search.Trim();

            query = query.Where(product =>
                product.Name.Contains(search) ||
                product.Description.Contains(search) ||
                product.Brand.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            string brand = filter.Brand.Trim();

            query = query.Where(product =>
                product.Brand.Contains(brand));
        }

        if (!string.IsNullOrWhiteSpace(
                filter.CategoryName))
        {
            string categoryName =
                filter.CategoryName.Trim();

            query = query.Where(product =>
                product.Category.Name == categoryName);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price <= filter.MaxPrice.Value);
        }

        if (filter.InStock.HasValue)
        {
            query = filter.InStock.Value
                ? query.Where(product =>
                    product.Stock > 0)
                : query.Where(product =>
                    product.Stock == 0);
        }

        string sortBy =
            filter.SortBy.Trim().ToLowerInvariant();

        bool descending =
            filter.SortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

        query = sortBy switch
        {
            "price" => descending
                ? query.OrderByDescending(
                    product => product.Price)
                : query.OrderBy(
                    product => product.Price),

            "stock" => descending
                ? query.OrderByDescending(
                    product => product.Stock)
                : query.OrderBy(
                    product => product.Stock),

            "createdat" => descending
                ? query.OrderByDescending(
                    product => product.CreatedAt)
                : query.OrderBy(
                    product => product.CreatedAt),

            _ => descending
                ? query.OrderByDescending(
                    product => product.Name)
                : query.OrderBy(
                    product => product.Name)
        };

        return await query
            .Take(filter.Limit)
            .Select(product => new ProductDto
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
                CategoryName = product.Category.Name
            })
            .ToListAsync(cancellationToken);
    }
}