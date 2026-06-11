using FoodOrderingLab2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingLab2.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Restaurant> Restaurants { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<RestaurantAttachment> RestaurantAttachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
                entity.HasIndex(e => e.AppUserId).IsUnique().HasFilter("[AppUserId] IS NOT NULL");
                entity.HasOne(e => e.AppUser)
                      .WithOne(u => u.Customer)
                      .HasForeignKey<Customer>(e => e.AppUserId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Customer)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.HasKey(e => e.RestaurantId);
                entity.Property(e => e.Rating).HasPrecision(3, 2);
                entity.HasMany(e => e.MenuItems)
                      .WithOne(m => m.Restaurant)
                      .HasForeignKey(m => m.RestaurantId);
                entity.HasMany(e => e.Orders)
                      .WithOne(o => o.Restaurant)
                      .HasForeignKey(o => o.RestaurantId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.MenuItemId);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.HasOne(e => e.Restaurant)
                      .WithMany(r => r.MenuItems)
                      .HasForeignKey(e => e.RestaurantId);
                entity.HasMany(e => e.OrderItems)
                      .WithOne(oi => oi.MenuItem)
                      .HasForeignKey(oi => oi.MenuItemId);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(e => e.CustomerId);
                entity.HasOne(e => e.Restaurant)
                      .WithMany(r => r.Orders)
                      .HasForeignKey(e => e.RestaurantId);
                entity.HasMany(e => e.OrderItems)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.HasOne(e => e.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(e => e.OrderId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.MenuItem)
                      .WithMany(mi => mi.OrderItems)
                      .HasForeignKey(e => e.MenuItemId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RestaurantAttachment>(entity =>
            {
                entity.HasKey(e => e.RestaurantAttachmentId);
                entity.HasOne(e => e.Restaurant)
                      .WithMany(r => r.Attachments)
                      .HasForeignKey(e => e.RestaurantId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
