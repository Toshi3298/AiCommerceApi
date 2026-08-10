using AiCommerceApi.Dtos.Products;
using MediatR;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProductById;

public sealed record GetAdminProductByIdQuery(
    int Id
) : IRequest<ProductDto?>;