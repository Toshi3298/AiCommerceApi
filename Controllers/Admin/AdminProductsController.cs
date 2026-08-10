using AiCommerceApi.Common.Responses;
using AiCommerceApi.Features.Admin.Products.Queries
    .GetAdminProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.Admin.Products.Queries.GetAdminProductById;

namespace AiCommerceApi.Controllers.Admin;

[ApiController]
[Route("api/admin/products")]
[Authorize(Roles = "Admin")]
public class AdminProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminProductsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] bool? isActive,
        [FromQuery] string? sortBy = "name",
        [FromQuery] string? sortDirection = "asc",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetAdminProductsQuery(
            search,
            brand,
            categoryId,
            minPrice,
            maxPrice,
            inStock,
            isActive,
            sortBy,
            sortDirection,
            pageNumber,
            pageSize);

        var products = await _mediator.Send(
            query,
            cancellationToken);

        var response =
            ApiResponse<object?>.Ok(
                products,
                "Admin ürün listesi başarıyla getirildi.");

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new GetAdminProductByIdQuery(id),
            cancellationToken);

        if (product is null)
        {
            var notFoundResponse =
                ApiResponse<ProductDto>.Fail(
                    "Ürün bulunamadı.");

            return NotFound(notFoundResponse);
        }

        var response =
            ApiResponse<ProductDto>.Ok(
                product,
                "Admin ürün detayı başarıyla getirildi.");

        return Ok(response);
    }
}