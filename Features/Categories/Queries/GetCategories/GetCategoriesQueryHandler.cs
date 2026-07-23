using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Data;
using AiCommerceApi.Dtos.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ApplicationDbContext _context;

    public GetCategoriesQueryHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            })
            .ToListAsync(cancellationToken);
    }
}