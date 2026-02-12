using Microsoft.EntityFrameworkCore;
using StudentManagement.Models;

namespace StudentManagement.Data
{
    /// <summary>
    /// DbContext cho StudentManagement
    /// Cấu hình kết nối database và entity mappings
    /// </summary>
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options) : base(options)
        {
        }

        // DbSet để truy vấn và thao tác với bảng Students
        public DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Students
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(e => e.StudentId);
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);
                
                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(true);
                
                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date");
                
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });
        }
    }
}
