using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AiCommerceApi.Features.Carts.Commands.AddCartItem;

public class AddCartItemResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public int? CartItemId { get; set; }

    public int Quantity { get; set; }
}