using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Entity đại diện cho bảng Books
    /// Quan hệ N-1 với Authors (N Books thuộc về 1 Author)
    /// </summary>
    [Table("Books")]
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Tên sách là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự")]
        [Display(Name = "Tên Sách")]
        public string BookTitle { get; set; } = string.Empty;

        [Display(Name = "Năm Xuất Bản")]
        [Range(1, 9999, ErrorMessage = "Năm xuất bản không hợp lệ")]
        public int? PublicationYear { get; set; }

        // Foreign Key
        [Required(ErrorMessage = "Tác giả là bắt buộc")]
        [Display(Name = "Tác Giả")]
        public int AuthorId { get; set; }

        // Navigation property: N-1 với Author
        [ForeignKey("AuthorId")]
        public virtual Author? Author { get; set; }
    }
}
