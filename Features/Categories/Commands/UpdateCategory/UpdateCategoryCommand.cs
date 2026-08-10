using MediatR;

namespace AiCommerceApi.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    int Id,
    string Name,
    string? Description
) : IRequest<UpdateCategoryResult>;