using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Categories;
using AiCommerceApi.Features.Categories.Commands.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Categories.Queries.GetCategories;


namespace AiCommerceApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(
        CreateCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description);

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
                message = "Kategori başarıyla oluşturuldu.",
                categoryId = result.CategoryId
            });
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _mediator.Send(
            new GetCategoriesQuery(),
            cancellationToken);

        return Ok(categories);
    }
}