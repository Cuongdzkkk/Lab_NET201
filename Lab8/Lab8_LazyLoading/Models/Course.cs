// Models/Course.cs
// Entity đại diện cho khóa học
// QUAN TRỌNG: Sử dụng từ khóa 'virtual' cho navigation properties

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8_LazyLoading.Models
{
    /// <summary>
    /// Khóa học - thuộc về 1 học sinh (đơn giản hóa cho demo)
    /// LƯU Ý: Navigation property phải là 'virtual' để Lazy Loading hoạt động
    /// </summary>
    public class Course
    {
        // Khóa chính
        [Key]
        public int CourseId { get; set; }

        // Tên khóa học
        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tên không được quá 200 ký tự")]
        [Display(Name = "Tên khóa học")]
        public string Title { get; set; } = string.Empty;

        // Mô tả khóa học
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        // Số tín chỉ
        [Display(Name = "Số tín chỉ")]
        public int Credits { get; set; } = 3;

        // Khóa ngoại liên kết với Student
        [Required]
        [Display(Name = "Học sinh")]
        public int StudentId { get; set; }

        // ========================================
        // LAZY LOADING: Navigation property với 'virtual'
        // ========================================
        // Từ khóa 'virtual' cho phép EF Core tạo proxy
        // Student sẽ được load tự động khi truy cập property này
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}
