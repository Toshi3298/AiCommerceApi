using AiCommerceApi.Common.Pagination;
using AiCommerceApi.Dtos.Products;
using MediatR;

namespace AiCommerceApi.Features.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? Search,
    string? Brand,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize
) : IRequest<PagedResult<ProductDto>>;