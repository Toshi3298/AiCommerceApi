using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        string productName = request.Name.Trim();
        string brand = request.Brand.Trim();

        if (string.IsNullOrWhiteSpace(productName))
        {
            return Failure("Ürün adı boş olamaz.");
        }

        if (string.IsNullOrWhiteSpace(brand))
        {
            return Failure("Marka boş olamaz.");
        }

        if (request.Price <= 0)
        {
            return Failure("Ürün fiyatı sıfırdan büyük olmalıdır.");
        }

        if (request.Stock < 0)
        {
            return Failure("Stok miktarı negatif olamaz.");
        }

        bool categoryExists =
            await _context.Categories.AnyAsync(
                category => category.Id == request.CategoryId,
                cancellationToken);

        if (!categoryExists)
        {
            return Failure("Kategori bulunamadı.");
        }

        var product = new Product
        {
            Name = productName,
            Description = request.Description.Trim(),
            Brand = brand,
            Price = request.Price,
            Stock = request.Stock,
            CategoryId = request.CategoryId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);

        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductResult
        {
            Success = true,
            ProductId = product.Id
        };
    }

    private static CreateProductResult Failure(string error)
    {
        return new CreateProductResult
        {
            Success = false,
            Error = error
        };
    }
}