using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly ApplicationDbContext _context;

    public GetProductsQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .AsQueryable();

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
                product.CategoryId == request.CategoryId.Value);
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

        return await query
            .OrderBy(product => product.Name)
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
                CategoryName = product.Category.Name
            })
            .ToListAsync(cancellationToken);
    }
}