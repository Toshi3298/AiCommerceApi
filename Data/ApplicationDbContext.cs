using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AiCommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Email tekrar edemez.
        modelBuilder.Entity<AppUser>()
            .HasIndex(user => user.Email)
            .IsUnique();

        // Bir kullanıcının yalnızca bir sepeti olabilir.
        modelBuilder.Entity<Cart>()
            .HasIndex(cart => cart.AppUserId)
            .IsUnique();

        modelBuilder.Entity<AppUser>()
            .HasOne(user => user.Cart)
            .WithOne(cart => cart.AppUser)
            .HasForeignKey<Cart>(cart => cart.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Kullanıcı - Sipariş ilişkisi
        modelBuilder.Entity<Order>()
            .HasOne(order => order.AppUser)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Kategori - Ürün ilişkisi
        modelBuilder.Entity<Product>()
            .HasOne(product => product.Category)
            .WithMany(category => category.Products)
            .HasForeignKey(product => product.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sepet - Sepet satırları ilişkisi
        modelBuilder.Entity<CartItem>()
            .HasOne(item => item.Cart)
            .WithMany(cart => cart.CartItems)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ürün - Sepet satırları ilişkisi
        modelBuilder.Entity<CartItem>()
            .HasOne(item => item.Product)
            .WithMany(product => product.CartItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Aynı ürün aynı sepette iki ayrı satır olmasın.
        modelBuilder.Entity<CartItem>()
            .HasIndex(item => new { item.CartId, item.ProductId })
            .IsUnique();

        // Sipariş - Sipariş satırları ilişkisi
        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.Order)
            .WithMany(order => order.OrderItems)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ürün - Sipariş satırları ilişkisi
        modelBuilder.Entity<OrderItem>()
            .HasOne(item => item.Product)
            .WithMany(product => product.OrderItems)
            .HasForeignKey(item => item.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Para alanlarının MSSQL türleri
        modelBuilder.Entity<Product>()
            .Property(product => product.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(order => order.TotalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(item => item.UnitPrice)
            .HasPrecision(18, 2);
    }
}