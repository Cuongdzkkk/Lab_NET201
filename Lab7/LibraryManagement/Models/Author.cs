using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Models
{
    /// <summary>
    /// Entity đại diện cho bảng Authors
    /// Quan hệ 1-N với Books (1 Author có nhiều Books)
    /// </summary>
    [Table("Authors")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
        [Display(Name = "Tên Tác Giả")]
        public string AuthorName { get; set; } = string.Empty;

        // Navigation property: 1 Author có nhiều Books
        public virtual ICollection<Book>? Books { get; set; }
    }
}
