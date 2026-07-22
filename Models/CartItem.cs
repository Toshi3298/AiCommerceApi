using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class CartItem
    {
    public int Id { get; set; }

    public int Quantity { get; set; }

    // Cart foreign key
    public int CartId { get; set; }

    public Cart Cart { get; set; } = null!;

    // Product foreign key
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;
    }
}
