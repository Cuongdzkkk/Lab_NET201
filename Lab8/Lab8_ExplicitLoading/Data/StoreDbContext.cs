// Data/StoreDbContext.cs
// DbContext cho hệ thống quản lý cửa hàng với EXPLICIT LOADING
// Explicit Loading: Load dữ liệu thủ công khi cần

using Lab8_ExplicitLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_ExplicitLoading.Data
{
    /// <summary>
    /// DbContext cho cửa hàng
    /// </summary>
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options)
            : base(options)
        {
        }

        // DbSet đại diện cho các bảng trong database
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Cấu hình Fluent API và Seed Data
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // CẤU HÌNH QUAN HỆ (Fluent API)
            // ========================================

            // Quan hệ Category - Product (1-N)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================
            // SEED DATA - Dữ liệu mẫu để test
            // ========================================

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Điện thoại",
                    Description = "Các loại điện thoại di động"
                },
                new Category
                {
                    CategoryId = 2,
                    CategoryName = "Laptop",
                    Description = "Máy tính xách tay các hãng"
                },
                new Category
                {
                    CategoryId = 3,
                    CategoryName = "Phụ kiện",
                    Description = "Phụ kiện điện tử các loại"
                },
                new Category
                {
                    CategoryId = 4,
                    CategoryName = "Tablet",
                    Description = "Máy tính bảng"
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                // Điện thoại
                new Product { ProductId = 1, Name = "iPhone 15 Pro Max", Price = 34990000, Stock = 50, CategoryId = 1 },
                new Product { ProductId = 2, Name = "Samsung Galaxy S24 Ultra", Price = 31990000, Stock = 45, CategoryId = 1 },
                new Product { ProductId = 3, Name = "Xiaomi 14 Ultra", Price = 22990000, Stock = 30, CategoryId = 1 },
                
                // Laptop
                new Product { ProductId = 4, Name = "MacBook Pro M3", Price = 52990000, Stock = 20, CategoryId = 2 },
                new Product { ProductId = 5, Name = "Dell XPS 15", Price = 42990000, Stock = 25, CategoryId = 2 },
                new Product { ProductId = 6, Name = "Asus ROG Zephyrus", Price = 55990000, Stock = 15, CategoryId = 2 },
                
                // Phụ kiện
                new Product { ProductId = 7, Name = "AirPods Pro 2", Price = 6490000, Stock = 100, CategoryId = 3 },
                new Product { ProductId = 8, Name = "Apple Watch Series 9", Price = 11990000, Stock = 60, CategoryId = 3 },
                new Product { ProductId = 9, Name = "Samsung Galaxy Buds3", Price = 4990000, Stock = 80, CategoryId = 3 },
                
                // Tablet
                new Product { ProductId = 10, Name = "iPad Pro M4", Price = 28990000, Stock = 35, CategoryId = 4 },
                new Product { ProductId = 11, Name = "Samsung Galaxy Tab S9", Price = 23990000, Stock = 40, CategoryId = 4 }
            );
        }
    }
}
