using FashionStoreAPI.Entities;
using FashionStoreAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace FashionStoreAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductVariant> ProductVariants { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<LikedProduct> LikedProducts { get; set; } = null!;
        public DbSet<ShoppingBasketItem> ShoppingBasketItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<RatingReminder> RatingReminders { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LikedProduct>()
                .HasKey(lp => new { lp.UserId, lp.ProductId });

            modelBuilder.Entity<ShoppingBasketItem>()
                .HasKey(sbi => new { sbi.UserId, sbi.ProductVariantId });

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductVariantId });

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Review)
                .WithOne(rv => rv.Rating)
                .HasForeignKey<Review>(rv => rv.RatingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
