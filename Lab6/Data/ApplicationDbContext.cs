using Microsoft.EntityFrameworkCore;
using Lab6.Models;

namespace Lab6.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed data - Students
        modelBuilder.Entity<Student>().HasData(
            new Student
            {
                StudentId = 1,
                StudentName = "Nguyễn Văn An",
                Email = "an.nv@test.com",
                PhoneNumber = "0901234567",
                DateOfBirth = new DateTime(2000, 1, 15),
                Address = "Quận 1, TP.HCM",
                GPA = 3.5m
            },
            new Student
            {
                StudentId = 2,
                StudentName = "Trần Thị Bình",
                Email = "binh.tt@test.com",
                PhoneNumber = "0912345678",
                DateOfBirth = new DateTime(1999, 5, 20),
                Address = "Quận Hoàn Kiếm, Hà Nội",
                GPA = 3.8m
            },
            new Student
            {
                StudentId = 3,
                StudentName = "Lê Minh Châu",
                Email = "chau.lm@test.com",
                PhoneNumber = "0923456789",
                DateOfBirth = new DateTime(2001, 3, 10),
                Address = "Quận 3, TP.HCM",
                GPA = 3.2m
            },
            new Student
            {
                StudentId = 4,
                StudentName = "Phạm Thị Dung",
                Email = "dung.pt@test.com",
                PhoneNumber = "0934567890",
                DateOfBirth = new DateTime(2000, 8, 25),
                Address = "Quận Đống Đa, Hà Nội",
                GPA = 3.9m
            },
            new Student
            {
                StudentId = 5,
                StudentName = "Hoàng Văn Em",
                Email = "em.hv@test.com",
                PhoneNumber = "0945678901",
                DateOfBirth = new DateTime(1998, 12, 5),
                Address = "Quận 7, TP.HCM",
                GPA = 3.6m
            }
        );

        // Seed data - Courses
        modelBuilder.Entity<Course>().HasData(
            new Course
            {
                CourseId = 1,
                CourseName = "Lập trình C# cơ bản",
                CourseCode = "CSC101",
                Credits = 3,
                Description = "Khóa học giới thiệu về ngôn ngữ lập trình C# và .NET Framework",
                StartDate = new DateTime(2024, 1, 15),
                EndDate = new DateTime(2024, 5, 15)
            },
            new Course
            {
                CourseId = 2,
                CourseName = "ASP.NET Core MVC",
                CourseCode = "WEB201",
                Credits = 4,
                Description = "Phát triển ứng dụng web với ASP.NET Core MVC",
                StartDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2024, 6, 1)
            },
            new Course
            {
                CourseId = 3,
                CourseName = "Cơ sở dữ liệu SQL Server",
                CourseCode = "DBS301",
                Credits = 3,
                Description = "Thiết kế và quản lý cơ sở dữ liệu với SQL Server",
                StartDate = new DateTime(2024, 1, 20),
                EndDate = new DateTime(2024, 5, 20)
            }
        );

        // Seed data - Enrollments
        modelBuilder.Entity<Enrollment>().HasData(
            new Enrollment
            {
                EnrollmentId = 1,
                StudentId = 1,
                CourseId = 1,
                EnrollmentDate = new DateTime(2024, 1, 10),
                Grade = "A"
            },
            new Enrollment
            {
                EnrollmentId = 2,
                StudentId = 1,
                CourseId = 2,
                EnrollmentDate = new DateTime(2024, 1, 25),
                Grade = "B"
            },
            new Enrollment
            {
                EnrollmentId = 3,
                StudentId = 2,
                CourseId = 1,
                EnrollmentDate = new DateTime(2024, 1, 10),
                Grade = "A"
            },
            new Enrollment
            {
                EnrollmentId = 4,
                StudentId = 2,
                CourseId = 3,
                EnrollmentDate = new DateTime(2024, 1, 15),
                Grade = "A"
            },
            new Enrollment
            {
                EnrollmentId = 5,
                StudentId = 3,
                CourseId = 2,
                EnrollmentDate = new DateTime(2024, 1, 28),
                Grade = "C"
            }
        );
    }
}
