using Microsoft.EntityFrameworkCore;
using Lab5.Models;

namespace Lab5.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<StudentDetails> StudentDetails { get; set; }
        
        // Sửa lại tên property DbSet cho chuẩn (không chữ h thừa)
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<ThanNhan> ThanNhans { get; set; } 
        
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDetails> CourseDetails { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ============================================
            // 1. NHAN VIEN - THAN NHAN (Fix Quan Trọng)
            // ============================================

            // A. Ép tên bảng SQL để không bị lỗi chính tả "ThanhNhans"
            modelBuilder.Entity<ThanNhan>().ToTable("ThanNhans");
            modelBuilder.Entity<NhanVien>().ToTable("NhanViens");

            // B. Thiết lập Composite Primary Key (Khóa chính gồm 2 cột) -> Sẽ hiện 2 chìa khóa vàng
            modelBuilder.Entity<ThanNhan>()
                .HasKey(t => new { t.MaNV, t.TenTN }); 

            // C. Thiết lập Quan hệ 1-N (HasOne - WithMany)
            // LƯU Ý: Phải đảm bảo trong Model NhanVien có property "ThanNhans" (không có chữ h ở giữa)
            modelBuilder.Entity<ThanNhan>()
                 .HasOne(t => t.NhanVien)
                 .WithMany(n => n.ThanNhans) 
                 .HasForeignKey(t => t.MaNV)
                 .OnDelete(DeleteBehavior.Cascade); // Xóa nhân viên xóa luôn người thân

            // D. Config Column Types (Data Types)
            modelBuilder.Entity<NhanVien>().Property(n => n.TenNV).HasColumnType("nvarchar(100)");
            modelBuilder.Entity<NhanVien>().Property(n => n.HoNV).HasColumnType("nvarchar(100)");
            modelBuilder.Entity<NhanVien>().Property(n => n.NgaySinh).HasColumnType("date");

            modelBuilder.Entity<ThanNhan>().Property(t => t.TenTN).HasColumnType("nvarchar(100)");
            modelBuilder.Entity<ThanNhan>().Property(t => t.NgSinh).HasColumnType("date");

            // ============================================
            // 2. SINH VIEN (1-1 & N-N)
            // ============================================

            // Bài 1: 1-1 (Student - StudentDetails)
            modelBuilder.Entity<StudentDetails>().HasKey(sd => sd.StudentId);
            modelBuilder.Entity<Student>()
                .HasOne(s => s.StudentDetails)
                .WithOne(sd => sd.Student)
                .HasForeignKey<StudentDetails>(sd => sd.StudentId);

            // Mapping types...
            modelBuilder.Entity<Student>().Property(s => s.FirstName).HasColumnType("varchar(100)");
            modelBuilder.Entity<StudentDetails>().Property(sd => sd.DateBirth).HasColumnType("date");

            // Bài 3: N-N (Student - Course) - Composite Key
            modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId);

            // ============================================
            // 4. COURSE - COURSEDETAILS (1-1)
            // ============================================
            
            modelBuilder.Entity<CourseDetails>()
                .HasKey(cd => cd.CourseDetailsId);
            
            modelBuilder.Entity<Course>()
                .HasOne(c => c.CourseDetails)
                .WithOne(cd => cd.Course)
                .HasForeignKey<CourseDetails>(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}