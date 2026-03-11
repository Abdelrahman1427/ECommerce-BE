using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ECommerceContext : IdentityDbContext<ApplicationUser>
    {
        public ECommerceContext(DbContextOptions<ECommerceContext> options) : base(options) { }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;

            // ─── Decimal precision ────────────────────────────────────────
            builder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            builder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.Discount).HasPrecision(18, 2);
            builder.Entity<Order>().Property(o => o.FinalAmount).HasPrecision(18, 2);

            builder.Entity<OrderItem>().Property(oi => oi.Price).HasPrecision(18, 2);
            builder.Entity<OrderItem>().Property(oi => oi.Total).HasPrecision(18, 2);

            builder.Entity<Discount>().Property(d => d.Value).HasPrecision(18, 2);
            builder.Entity<Discount>().Property(d => d.MinimumOrderTotal).HasPrecision(18, 2);

            // ─── Unique indexes ───────────────────────────────────────────
            builder.Entity<Order>().HasIndex(o => o.OrderNumber).IsUnique();
            builder.Entity<Discount>().HasIndex(d => d.Code).IsUnique();

            // ─── Enums as string ──────────────────────────────────────────
            builder.Entity<Order>().Property(o => o.Status).HasConversion<string>();
            builder.Entity<OrderStatusHistory>().Property(h => h.Status).HasConversion<string>();
            builder.Entity<Discount>().Property(d => d.Type).HasConversion<string>();
            builder.Entity<Notification>().Property(n => n.Type).HasConversion<string>();

            // ─── Relationships ────────────────────────────────────────────

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrderStatusHistory>()
                .HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
