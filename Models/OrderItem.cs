using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class OrderItem
    {
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    // Order foreign key
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    // Product foreign key
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;   
    }
}
