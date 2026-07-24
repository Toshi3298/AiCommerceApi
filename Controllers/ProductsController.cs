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
        CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(
            new GetProductsQuery(),
            cancellationToken);

        var response =
            ApiResponse<List<ProductDto>>.Ok(
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