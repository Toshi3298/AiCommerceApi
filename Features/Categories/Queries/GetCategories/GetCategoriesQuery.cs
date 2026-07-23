using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Categories;
using MediatR;

namespace AiCommerceApi.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery
    : IRequest<List<CategoryDto>>;