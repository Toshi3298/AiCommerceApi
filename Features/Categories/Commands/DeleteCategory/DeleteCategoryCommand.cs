using MediatR;

namespace AiCommerceApi.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(
    int Id
) : IRequest<DeleteCategoryResult>;