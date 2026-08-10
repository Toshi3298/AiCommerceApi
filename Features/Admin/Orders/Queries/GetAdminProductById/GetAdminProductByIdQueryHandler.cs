using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProductById;

public sealed class GetAdminProductByIdQueryHandler
    : IRequestHandler<
        GetAdminProductByIdQuery,
        ProductDto?>
{
    private readonly ApplicationDbContext _context;

    public GetAdminProductByIdQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto?> Handle(
        GetAdminProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product =>
                product.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);
    }
}