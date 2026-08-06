using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AiCommerceApi.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string Brand,
    decimal Price,
    int Stock,
    int CategoryId,
    string? ImageUrl

) : IRequest<CreateProductResult>;