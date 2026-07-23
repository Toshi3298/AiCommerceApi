using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Products;
using MediatR;

namespace AiCommerceApi.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id)
    : IRequest<ProductDto?>;