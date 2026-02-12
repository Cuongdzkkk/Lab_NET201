// Data/AcademyDbContext.cs
// DbContext cho hệ thống quản lý học sinh nâng cao với COMBINED LOADING
// Bao gồm cấu hình Many-to-Many và Seed Data

using Lab8_CombinedLoading.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab8_CombinedLoading.Data
{
    /// <summary>
    /// DbContext cho Academy - demo tất cả các loại loading
    /// </summary>
    public class AcademyDbContext : DbContext
    {
        public AcademyDbContext(DbContextOptions<AcademyDbContext> options)
            : base(options)
        {
        }

        // DbSet đại diện cho các bảng trong database
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        /// <summary>
        /// Cấu hình Fluent API và Seed Data
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // CẤU HÌNH QUAN HỆ MANY-TO-MANY
            // ========================================

            // Quan hệ Student - Enrollment (1-N)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Course - Enrollment (1-N)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index duy nhất để 1 student không đăng ký 1 course 2 lần
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            // ========================================
            // SEED DATA - Dữ liệu mẫu để test
            // ========================================

            // Seed Students
            modelBuilder.Entity<Student>().HasData(
                new Student { StudentId = 1, Name = "Nguyễn Văn An", Email = "an.nguyen@email.com", Class = "CNTT-K65" },
                new Student { StudentId = 2, Name = "Trần Thị Bình", Email = "binh.tran@email.com", Class = "CNTT-K65" },
                new Student { StudentId = 3, Name = "Lê Hoàng Cường", Email = "cuong.le@email.com", Class = "CNTT-K66" },
                new Student { StudentId = 4, Name = "Phạm Thu Dung", Email = "dung.pham@email.com", Class = "CNTT-K66" },
                new Student { StudentId = 5, Name = "Hoàng Minh Đức", Email = "duc.hoang@email.com", Class = "CNTT-K67" }
            );

            // Seed Courses
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, Title = "Lập trình C#", Credits = 4, Instructor = "ThS. Nguyễn Văn A" },
                new Course { CourseId = 2, Title = "Cơ sở dữ liệu", Credits = 3, Instructor = "TS. Trần Thị B" },
                new Course { CourseId = 3, Title = "ASP.NET Core", Credits = 4, Instructor = "ThS. Lê Văn C" },
                new Course { CourseId = 4, Title = "Entity Framework", Credits = 3, Instructor = "ThS. Lê Văn C" },
                new Course { CourseId = 5, Title = "Phát triển Web", Credits = 3, Instructor = "ThS. Phạm Văn D" }
            );

            // Seed Enrollments - Many-to-Many relationships
            modelBuilder.Entity<Enrollment>().HasData(
                // Nguyễn Văn An đăng ký 3 môn
                new Enrollment { EnrollmentId = 1, StudentId = 1, CourseId = 1, EnrollmentDate = new DateTime(2024, 1, 10), Grade = 8.5m },
                new Enrollment { EnrollmentId = 2, StudentId = 1, CourseId = 2, EnrollmentDate = new DateTime(2024, 1, 10), Grade = 7.0m },
                new Enrollment { EnrollmentId = 3, StudentId = 1, CourseId = 3, EnrollmentDate = new DateTime(2024, 1, 15) },

                // Trần Thị Bình đăng ký 4 môn
                new Enrollment { EnrollmentId = 4, StudentId = 2, CourseId = 1, EnrollmentDate = new DateTime(2024, 1, 10), Grade = 9.0m },
                new Enrollment { EnrollmentId = 5, StudentId = 2, CourseId = 2, EnrollmentDate = new DateTime(2024, 1, 10), Grade = 8.0m },
                new Enrollment { EnrollmentId = 6, StudentId = 2, CourseId = 4, EnrollmentDate = new DateTime(2024, 1, 12), Grade = 9.5m },
                new Enrollment { EnrollmentId = 7, StudentId = 2, CourseId = 5, EnrollmentDate = new DateTime(2024, 1, 15) },

                // Lê Hoàng Cường đăng ký 2 môn
                new Enrollment { EnrollmentId = 8, StudentId = 3, CourseId = 3, EnrollmentDate = new DateTime(2024, 1, 11), Grade = 7.5m },
                new Enrollment { EnrollmentId = 9, StudentId = 3, CourseId = 4, EnrollmentDate = new DateTime(2024, 1, 11), Grade = 8.0m },

                // Phạm Thu Dung đăng ký 3 môn
                new Enrollment { EnrollmentId = 10, StudentId = 4, CourseId = 1, EnrollmentDate = new DateTime(2024, 1, 12), Grade = 6.5m },
                new Enrollment { EnrollmentId = 11, StudentId = 4, CourseId = 3, EnrollmentDate = new DateTime(2024, 1, 12) },
                new Enrollment { EnrollmentId = 12, StudentId = 4, CourseId = 5, EnrollmentDate = new DateTime(2024, 1, 14) },

                // Hoàng Minh Đức đăng ký 2 môn
                new Enrollment { EnrollmentId = 13, StudentId = 5, CourseId = 2, EnrollmentDate = new DateTime(2024, 1, 13), Grade = 8.5m },
                new Enrollment { EnrollmentId = 14, StudentId = 5, CourseId = 5, EnrollmentDate = new DateTime(2024, 1, 13) }
            );
        }
    }
}
