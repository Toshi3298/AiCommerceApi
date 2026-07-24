using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly ApplicationDbContext _context;

    public DeleteProductCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DeleteProductResult> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(
                product => product.Id == request.Id,
                cancellationToken);

        if (product is null)
        {
            return new DeleteProductResult
            {
                Success = false,
                NotFound = true,
                Error = "Ürün bulunamadı."
            };
        }

        product.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult
        {
            Success = true
        };
    }
}