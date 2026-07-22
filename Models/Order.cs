using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class Order
    {
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public decimal TotalPrice { get; set; }

    public string ShippingAddress { get; set; } = string.Empty;

    // Foreign key
    public int AppUserId { get; set; }

    // Navigation property
    public AppUser AppUser { get; set; } = null!;

    // Bir siparişte birçok ürün satırı bulunabilir.
    public List<OrderItem> OrderItems { get; set; } = new();  
    }
}
