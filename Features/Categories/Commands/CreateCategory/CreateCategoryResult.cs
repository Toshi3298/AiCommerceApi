using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryResult
    {
    public bool Success { get; set; }

    public int? CategoryId { get; set; }

    public string? Error { get; set; }  
    }
}