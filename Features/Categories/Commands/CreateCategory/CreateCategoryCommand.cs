using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

namespace AiCommerceApi.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string? Description
) : IRequest<CreateCategoryResult>;