using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Products;
using MediatR;

namespace AiCommerceApi.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? Search,
    string? Brand,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock
)
    : IRequest<List<ProductDto>>;