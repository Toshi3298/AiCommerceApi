using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        return await _context.Products
            .AsNoTracking()
            .Where(product =>product.IsActive)
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