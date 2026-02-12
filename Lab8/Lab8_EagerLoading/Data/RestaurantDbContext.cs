// Data/RestaurantDbContext.cs
// DbContext cho hệ thống đặt hàng nhà hàng
// Bao gồm: Cấu hình quan hệ và Seed Data mẫu

using Lab8_EagerLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_EagerLoading.Data
{
    /// <summary>
    /// DbContext quản lý kết nối và các entity của nhà hàng
    /// </summary>
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options)
            : base(options)
        {
        }

        // DbSet đại diện cho các bảng trong database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Dish> Dishes { get; set; }

        /// <summary>
        /// Cấu hình Fluent API và Seed Data
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // CẤU HÌNH QUAN HỆ (Fluent API)
            // ========================================

            // Quan hệ Customer - Order (1-N)
            // 1 Customer có nhiều Orders
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Customer sẽ xóa luôn các Orders

            // Quan hệ Order - Dish (1-N)
            // 1 Order có nhiều Dishes
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Dishes)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Order sẽ xóa luôn các Dishes

            // ========================================
            // SEED DATA - Dữ liệu mẫu để test
            // ========================================

            // Seed Customers
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    Name = "Nguyễn Văn An",
                    Phone = "0901234567"
                },
                new Customer
                {
                    CustomerId = 2,
                    Name = "Trần Thị Bích",
                    Phone = "0912345678"
                },
                new Customer
                {
                    CustomerId = 3,
                    Name = "Lê Văn Cường",
                    Phone = "0923456789"
                }
            );

            // Seed Orders
            modelBuilder.Entity<Order>().HasData(
                // Đơn hàng của khách Nguyễn Văn An
                new Order
                {
                    OrderId = 1,
                    CustomerId = 1,
                    OrderDate = new DateTime(2024, 1, 15, 12, 30, 0),
                    TotalAmount = 350000,
                    Status = "Hoàn thành"
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = 1,
                    OrderDate = new DateTime(2024, 1, 20, 18, 45, 0),
                    TotalAmount = 520000,
                    Status = "Hoàn thành"
                },
                // Đơn hàng của khách Trần Thị Bích
                new Order
                {
                    OrderId = 3,
                    CustomerId = 2,
                    OrderDate = new DateTime(2024, 1, 18, 19, 00, 0),
                    TotalAmount = 280000,
                    Status = "Đang giao"
                },
                // Đơn hàng của khách Lê Văn Cường
                new Order
                {
                    OrderId = 4,
                    CustomerId = 3,
                    OrderDate = new DateTime(2024, 1, 22, 12, 00, 0),
                    TotalAmount = 450000,
                    Status = "Đang xử lý"
                }
            );

            // Seed Dishes
            modelBuilder.Entity<Dish>().HasData(
                // Món ăn trong đơn hàng 1
                new Dish { DishId = 1, Name = "Phở bò đặc biệt", Price = 75000, Quantity = 2, OrderId = 1 },
                new Dish { DishId = 2, Name = "Nem rán", Price = 50000, Quantity = 1, OrderId = 1 },
                new Dish { DishId = 3, Name = "Nước cam ép", Price = 35000, Quantity = 2, OrderId = 1 },
                
                // Món ăn trong đơn hàng 2
                new Dish { DishId = 4, Name = "Bún chả Hà Nội", Price = 85000, Quantity = 2, OrderId = 2 },
                new Dish { DishId = 5, Name = "Chả giò", Price = 60000, Quantity = 1, OrderId = 2 },
                new Dish { DishId = 6, Name = "Cơm rang dưa bò", Price = 95000, Quantity = 1, OrderId = 2 },
                new Dish { DishId = 7, Name = "Trà đá", Price = 10000, Quantity = 3, OrderId = 2 },
                
                // Món ăn trong đơn hàng 3
                new Dish { DishId = 8, Name = "Bún bò Huế", Price = 80000, Quantity = 2, OrderId = 3 },
                new Dish { DishId = 9, Name = "Gỏi cuốn", Price = 45000, Quantity = 1, OrderId = 3 },
                
                // Món ăn trong đơn hàng 4
                new Dish { DishId = 10, Name = "Lẩu thái", Price = 250000, Quantity = 1, OrderId = 4 },
                new Dish { DishId = 11, Name = "Cơm chiên hải sản", Price = 120000, Quantity = 1, OrderId = 4 },
                new Dish { DishId = 12, Name = "Sinh tố bơ", Price = 40000, Quantity = 2, OrderId = 4 }
            );
        }
    }
}
