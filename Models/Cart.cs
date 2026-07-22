using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class Cart
    {
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Foreign key
    public int AppUserId { get; set; }

    // Navigation property
    public AppUser AppUser { get; set; } = null!;

    // Bir sepette birçok ürün satırı bulunabilir.
    public List<CartItem> CartItems { get; set; } = new();   
    }
}
