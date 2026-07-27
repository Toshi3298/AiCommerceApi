using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.Products.Commands.CreateProduct;
using AiCommerceApi.Features.Products.Commands.DeleteProduct;
using AiCommerceApi.Features.Products.Commands.UpdateProduct;
using AiCommerceApi.Features.Products.Queries.GetProductById;
using AiCommerceApi.Features.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCommerceApi.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.Description,
            request.Brand,
            request.Price,
            request.Stock,
            request.CategoryId);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün oluşturulamadı.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    productId = result.ProductId
                },
                "Ürün başarıyla oluşturuldu.");

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? search,
        [FromQuery] string? brand,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? inStock,
        [FromQuery] string? sortBy = "name",
        [FromQuery] string? sortDirection = "asc",
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (minPrice.HasValue && minPrice.Value < 0)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Minimum fiyat negatif olamaz."));
        }

        if (maxPrice.HasValue && maxPrice.Value < 0)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Maksimum fiyat negatif olamaz."));
        }

        if (minPrice.HasValue &&
            maxPrice.HasValue &&
            minPrice.Value > maxPrice.Value)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Minimum fiyat maksimum fiyattan büyük olamaz."));
        }

        if (categoryId.HasValue && categoryId.Value <= 0)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Kategori ID sıfırdan büyük olmalıdır."));
        }

        if (pageNumber <= 0)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Sayfa numarası sıfırdan büyük olmalıdır."));
        }

        if (pageSize <= 0 || pageSize > 100)
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Sayfa boyutu 1 ile 100 arasında olmalıdır."));
        }

        string[] allowedSortFields =
        {
            "name",
            "price",
            "stock",
            "createdat"
        };

        string normalizedSortBy =
            sortBy?.Trim().ToLowerInvariant() ?? "name";

        if (!allowedSortFields.Contains(normalizedSortBy))
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Sıralama alanı name, price, stock " +
                    "veya createdAt olmalıdır."));
        }

        string normalizedSortDirection =
            sortDirection?.Trim().ToLowerInvariant() ?? "asc";

        if (normalizedSortDirection is not "asc" and not "desc")
        {
            return BadRequest(
                ApiResponse<object?>.Fail(
                    "Sıralama yönü asc veya desc olmalıdır."));
        }

        var query = new GetProductsQuery(
            search,
            brand,
            categoryId,
            minPrice,
            maxPrice,
            inStock,
            normalizedSortBy,
            normalizedSortDirection,
            pageNumber,
            pageSize);

        var products = await _mediator.Send(
            query,
            cancellationToken);

        var response =
            ApiResponse<object?>.Ok(
                products,
                "Ürünler başarıyla getirildi.");

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductById(
        int id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        if (product is null)
        {
            var errorResponse =
                ApiResponse<ProductDto>.Fail(
                    "Ürün bulunamadı.");

            return NotFound(errorResponse);
        }

        var response =
            ApiResponse<ProductDto>.Ok(
                product,
                "Ürün başarıyla getirildi.");

        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateProduct(
        int id,
        UpdateProductRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.Brand,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.IsActive);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (result.NotFound)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün bulunamadı.");

            return NotFound(errorResponse);
        }

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün güncellenemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                null,
                "Ürün başarıyla güncellendi.");

        return Ok(response);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProduct(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteProductCommand(id),
            cancellationToken);

        if (result.NotFound)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün bulunamadı.");

            return NotFound(errorResponse);
        }

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Ürün pasife alınamadı.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                null,
                "Ürün başarıyla pasife alındı.");

        return Ok(response);
    }
}