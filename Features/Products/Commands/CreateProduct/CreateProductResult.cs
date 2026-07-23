using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Features.Products.Commands.CreateProduct
{
    public class CreateProductResult
    {
    public bool Success { get; set; }

    public int? ProductId { get; set; }

    public string? Error { get; set; }   
    }
}