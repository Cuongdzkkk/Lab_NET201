// Data/SchoolDbContext.cs
// DbContext cho hệ thống quản lý học sinh với LAZY LOADING
// QUAN TRỌNG: Cấu hình UseLazyLoadingProxies() trong Program.cs

using Lab8_LazyLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_LazyLoading.Data
{
    /// <summary>
    /// DbContext cho trường học - sử dụng Lazy Loading
    /// </summary>
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        // DbSet đại diện cho các bảng trong database
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }

        /// <summary>
        /// Cấu hình Fluent API và Seed Data
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // CẤU HÌNH QUAN HỆ (Fluent API)
            // ========================================

            // Quan hệ Student - Course (1-N)
            // 1 Student có nhiều Courses
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Student)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // ========================================
            // SEED DATA - Dữ liệu mẫu để test Lazy Loading
            // ========================================

            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1,
                    Name = "Nguyễn Minh Tuấn",
                    Email = "tuan.nguyen@email.com",
                    DateOfBirth = new DateTime(2000, 5, 15)
                },
                new Student
                {
                    StudentId = 2,
                    Name = "Trần Thu Hà",
                    Email = "ha.tran@email.com",
                    DateOfBirth = new DateTime(2001, 3, 20)
                },
                new Student
                {
                    StudentId = 3,
                    Name = "Lê Văn Đức",
                    Email = "duc.le@email.com",
                    DateOfBirth = new DateTime(1999, 11, 8)
                },
                new Student
                {
                    StudentId = 4,
                    Name = "Phạm Thị Mai",
                    Email = "mai.pham@email.com",
                    DateOfBirth = new DateTime(2002, 7, 25)
                }
            );

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                // Khóa học của Nguyễn Minh Tuấn
                new Course { CourseId = 1, Title = "Lập trình C#", Description = "Cơ bản về C# và .NET", Credits = 4, StudentId = 1 },
                new Course { CourseId = 2, Title = "ASP.NET Core MVC", Description = "Xây dựng web với ASP.NET", Credits = 3, StudentId = 1 },
                new Course { CourseId = 3, Title = "Entity Framework Core", Description = "ORM cho .NET", Credits = 3, StudentId = 1 },
                
                // Khóa học của Trần Thu Hà
                new Course { CourseId = 4, Title = "Python cơ bản", Description = "Nhập môn Python", Credits = 3, StudentId = 2 },
                new Course { CourseId = 5, Title = "Machine Learning", Description = "ML với Python", Credits = 4, StudentId = 2 },
                
                // Khóa học của Lê Văn Đức
                new Course { CourseId = 6, Title = "JavaScript", Description = "JS cơ bản đến nâng cao", Credits = 3, StudentId = 3 },
                new Course { CourseId = 7, Title = "React.js", Description = "Frontend với React", Credits = 4, StudentId = 3 },
                new Course { CourseId = 8, Title = "Node.js", Description = "Backend với Node", Credits = 3, StudentId = 3 },
                
                // Khóa học của Phạm Thị Mai
                new Course { CourseId = 9, Title = "SQL Server", Description = "Quản trị cơ sở dữ liệu", Credits = 3, StudentId = 4 },
                new Course { CourseId = 10, Title = "MongoDB", Description = "NoSQL Database", Credits = 3, StudentId = 4 }
            );
        }
    }
}
