using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Dtos.Carts
{
    public class AddCartItemRequestDto
    {
    public int ProductId { get; set; }

    public int Quantity { get; set; }
    }
}