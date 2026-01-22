using Microsoft.EntityFrameworkCore;
using Lab4.Models;

namespace Lab4.Data
{
    public class CompanyDbContext : DbContext
    {
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) 
        : base(options)
        {
        }

        public DbSet<Company> Companies {get;set;}
        public DbSet<Department> Departments {get;set;}
        public DbSet<Employee> Employees {get;set;}
        public DbSet<Product> Products {get;set;} 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ▣ SEED 3 DEPARTMENTS
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, Name = "IT Department" },
                new Department { Id = 2, Name = "HR Department" },
                new Department { Id = 3, Name = "Marketing Department" }
            );

            // ▣ SEED 6 EMPLOYEES (DISTRIBUTED ACROSS DEPARTMENTS)
            modelBuilder.Entity<Employee>().HasData(
                // IT Department
                new Employee 
                { 
                    Id = 1, 
                    Name = "Nguyễn Văn An", 
                    Designation = "Senior Developer", 
                    DepartmentId = 1 
                },
                new Employee 
                { 
                    Id = 2, 
                    Name = "Trần Thị Bình", 
                    Designation = "DevOps Engineer", 
                    DepartmentId = 1 
                },
                
                // HR Department
                new Employee 
                { 
                    Id = 3, 
                    Name = "Lê Minh Cường", 
                    Designation = "HR Manager", 
                    DepartmentId = 2 
                },
                new Employee 
                { 
                    Id = 4, 
                    Name = "Phạm Thu Dung", 
                    Designation = "Recruiter", 
                    DepartmentId = 2 
                },
                
                // Marketing Department
                new Employee 
                { 
                    Id = 5, 
                    Name = "Hoàng Văn Đức", 
                    Designation = "Marketing Director", 
                    DepartmentId = 3 
                },
                new Employee 
                { 
                    Id = 6, 
                    Name = "Vũ Thị Hà", 
                    Designation = "Content Creator", 
                    DepartmentId = 3 
                }
            );

            // 🚀 SEED 10 PRODUCTS (VARIED FOR FILTER TESTING)
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Dell Gaming G15", Price = 25000000, Quantity = 5, Status = true, ImagePath = "" },
                new Product { Id = 2, Name = "Macbook Pro M3", Price = 45000000, Quantity = 3, Status = true, ImagePath = "" },
                new Product { Id = 3, Name = "Logitech MX Master 3", Price = 2500000, Quantity = 100, Status = true, ImagePath = "" },
                new Product { Id = 4, Name = "Samsung Galaxy S24 Ultra", Price = 30000000, Quantity = 8, Status = true, ImagePath = "" },
                new Product { Id = 5, Name = "iPhone 15 Pro Max", Price = 35000000, Quantity = 12, Status = true, ImagePath = "" },
                new Product { Id = 6, Name = "Sony WH-1000XM5", Price = 8000000, Quantity = 20, Status = true, ImagePath = "" },
                new Product { Id = 7, Name = "Keychron Q1 Pro", Price = 4500000, Quantity = 15, Status = true, ImagePath = "" },
                new Product { Id = 8, Name = "LG UltraGear Monitor", Price = 9000000, Quantity = 7, Status = true, ImagePath = "" },
                new Product { Id = 9, Name = "Razer DeathAdder V3", Price = 1500000, Quantity = 50, Status = true, ImagePath = "" },
                new Product { Id = 10, Name = "Asus ROG Zephyrus G14", Price = 42000000, Quantity = 4, Status = true, ImagePath = "" }
            );
        }
    }
}