using AiCommerceApi.Dtos.Categories;
using AiCommerceApi.Features.Categories.Commands.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Categories.Queries.GetCategories;
using AiCommerceApi.Common.Responses;


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
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Kategori oluşturulamadı.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    categoryId = result.CategoryId
                },
                "Kategori başarıyla oluşturuldu.");

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(
        CancellationToken cancellationToken)
    {
        var categories = await _mediator.Send(
            new GetCategoriesQuery(),
            cancellationToken);

        var response =
            ApiResponse<List<CategoryDto>>.Ok(
                categories,
                "Kategoriler başarıyla getirildi.");

        return Ok(response);
    }
}