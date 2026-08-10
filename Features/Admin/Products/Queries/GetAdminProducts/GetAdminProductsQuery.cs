using AiCommerceApi.Common.Pagination;
using AiCommerceApi.Dtos.Products;
using MediatR;

namespace AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProducts;

public record GetAdminProductsQuery(
    string? Search,
    string? Brand,
    int? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? InStock,
    bool? IsActive,
    string? SortBy,
    string? SortDirection,
    int PageNumber,
    int PageSize
) : IRequest<PagedResult<ProductDto>>;