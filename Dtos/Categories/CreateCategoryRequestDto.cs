using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Dtos.Categories
{
    public class CreateCategoryRequestDto
    {
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
    }
}