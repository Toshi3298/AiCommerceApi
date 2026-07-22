using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public class AppUser
    {
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Bir kullanıcının bir sepeti olabilir.
    public Cart? Cart { get; set; }

    // Bir kullanıcı birçok sipariş verebilir.
    public List<Order> Orders { get; set; } = new();
    }
}
