using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateProductCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProductResult> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(
            product => product.Id == request.Id,
            cancellationToken);

        if (product is null)
        {
            return new UpdateProductResult
            {
                Success = false,
                NotFound = true,
                Error = "Ürün bulunamadı."
            };
        }

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

        product.Name = productName;
        product.Description = request.Description.Trim();
        product.Brand = brand;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.CategoryId = request.CategoryId;
        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult
        {
            Success = true
        };
    }

    private static UpdateProductResult Failure(string error)
    {
        return new UpdateProductResult
        {
            Success = false,
            Error = error
        };
    }
}