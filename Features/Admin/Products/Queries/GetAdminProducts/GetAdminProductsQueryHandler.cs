using AiCommerceApi.Common.Pagination;
using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProducts;

public class GetAdminProductsQueryHandler
    : IRequestHandler<
        GetAdminProductsQuery,
        PagedResult<ProductDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAdminProductsQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        GetAdminProductsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Product> query =
            _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string search = request.Search.Trim();

            query = query.Where(product =>
                product.Name.Contains(search) ||
                product.Description.Contains(search) ||
                product.Brand.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(request.Brand))
        {
            string brand = request.Brand.Trim();

            query = query.Where(product =>
                product.Brand.Contains(brand));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(product =>
                product.CategoryId ==
                request.CategoryId.Value);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(product =>
                product.Price <= request.MaxPrice.Value);
        }

        if (request.InStock.HasValue)
        {
            query = request.InStock.Value
                ? query.Where(product => product.Stock > 0)
                : query.Where(product => product.Stock == 0);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(product =>
                product.IsActive ==
                request.IsActive.Value);
        }

        int totalCount =
            await query.CountAsync(cancellationToken);

        string sortBy =
            request.SortBy?.Trim().ToLowerInvariant()
            ?? "name";

        bool descending =
            request.SortDirection?.Trim()
                .Equals(
                    "desc",
                    StringComparison.OrdinalIgnoreCase)
            ?? false;

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

        var products = await query
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
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

        return new PagedResult<ProductDto>
        {
            Items = products,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            TotalPages = (int)Math.Ceiling(
                totalCount /
                (double)request.PageSize)
        };
    }
}   