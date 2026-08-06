using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class Product
    {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }


    public decimal Price { get; set; }

    public int Stock { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int CategoryId { get; set; }

    // Navigation property
    public Category Category { get; set; } = null!;

    // Ürün farklı sepetlerde bulunabilir.
    public List<CartItem> CartItems { get; set; } = new();

    // Ürün farklı siparişlerde bulunabilir.
    public List<OrderItem> OrderItems { get; set; } = new();
    }
}
