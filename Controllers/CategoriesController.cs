using AiCommerceApi.Dtos.Categories;
using AiCommerceApi.Features.Categories.Commands.CreateCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Categories.Queries.GetCategories;
using AiCommerceApi.Common.Responses;
using AiCommerceApi.Features.Categories.Commands.UpdateCategory;
using AiCommerceApi.Features.Categories.Commands.DeleteCategory;

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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        UpdateCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
    var command = new UpdateCategoryCommand(
        id,
        request.Name,
        request.Description);

    var result = await _mediator.Send(
        command,
        cancellationToken);

    if (result.NotFound)
    {
        var notFoundResponse =
            ApiResponse<object?>.Fail(
                result.Error
                ?? "Kategori bulunamadı.");

        return NotFound(notFoundResponse);
    }

    if (!result.Success)
    {
        var errorResponse =
            ApiResponse<object?>.Fail(
                result.Error
                ?? "Kategori güncellenemedi.");

        return BadRequest(errorResponse);
    }

    var response =
        ApiResponse<object?>.Ok(
            null,
            "Kategori başarıyla güncellendi.");

    return Ok(response);
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new DeleteCategoryCommand(id),
            cancellationToken);

        if (result.NotFound)
        {
            var notFoundResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Kategori bulunamadı.");

            return NotFound(notFoundResponse);
        }

        if (result.InUse)
        {
            var conflictResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Kategori kullanımda olduğu için silinemedi.");

            return Conflict(conflictResponse);
        }

        if (!result.Success)
        {
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Kategori silinemedi.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                null,
                "Kategori başarıyla silindi.");

        return Ok(response);
    }    
}