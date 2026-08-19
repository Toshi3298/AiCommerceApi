using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AiCommerceApi.Dtos.Carts;


public class CartResponseDto
{
    public int? CartId { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();

    public int TotalQuantity { get; set; }

    public decimal TotalPrice { get; set; }
}