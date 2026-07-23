using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Products;
using AiCommerceApi.Features.Products.Commands.CreateProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Products.Queries.GetProducts;
namespace AiCommerceApi.Controllers;
using AiCommerceApi.Features.Products.Queries.GetProductById;

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
            return BadRequest(new
            {
                message = result.Error
            });
        }

        return StatusCode(
            StatusCodes.Status201Created,
            new
            {
                message = "Ürün başarıyla oluşturuldu.",
                productId = result.ProductId
            });
    }
    [HttpGet]
    public async Task<IActionResult> GetProducts(
    CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(
            new GetProductsQuery(),
            cancellationToken);

        return Ok(products);
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
            return NotFound(new
            {
                message = "Ürün bulunamadı."
            });
        }

        return Ok(product);
}
}