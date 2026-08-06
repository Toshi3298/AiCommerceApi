using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Dtos.Products
{
    public class UpdateProductRequestDto
    {
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    }
}