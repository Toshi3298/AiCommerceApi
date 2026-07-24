using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AiCommerceApi.Features.Products.Commands.DeleteProduct;

public class DeleteProductResult
{
    public bool Success { get; set; }

    public bool NotFound { get; set; }

    public string? Error { get; set; }
}