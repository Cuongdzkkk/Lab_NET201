using Microsoft.EntityFrameworkCore;
using LibraryManagement.Models;

namespace LibraryManagement.Data
{
    /// <summary>
    /// DbContext cho LibraryManagement
    /// Cấu hình kết nối database và entity mappings
    /// Sử dụng Fluent API để cấu hình relationships
    /// </summary>
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        // DbSets cho LINQ to Entities queries
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng Authors
            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Authors");
                entity.HasKey(e => e.AuthorId);
                
                entity.Property(e => e.AuthorName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(true);
            });

            // Cấu hình bảng Books
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.BookId);
                
                entity.Property(e => e.BookTitle)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(true);

                // Cấu hình quan hệ FK với Author
                entity.HasOne(b => b.Author)
                    .WithMany(a => a.Books)
                    .HasForeignKey(b => b.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Author nếu có Books
            });

            // Seed dữ liệu mẫu
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, AuthorName = "Nguyễn Nhật Ánh" },
                new Author { AuthorId = 2, AuthorName = "Nam Cao" },
                new Author { AuthorId = 3, AuthorName = "Tô Hoài" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, BookTitle = "Mắt Biếc", PublicationYear = 1990, AuthorId = 1 },
                new Book { BookId = 2, BookTitle = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh", PublicationYear = 2010, AuthorId = 1 },
                new Book { BookId = 3, BookTitle = "Chí Phèo", PublicationYear = 1941, AuthorId = 2 },
                new Book { BookId = 4, BookTitle = "Dế Mèn Phiêu Lưu Ký", PublicationYear = 1941, AuthorId = 3 }
            );
        }
    }
}
