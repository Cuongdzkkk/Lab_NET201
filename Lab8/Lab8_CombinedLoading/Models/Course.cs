// Models/Course.cs
// Entity khóa học trong hệ thống quản lý nâng cao
// Quan hệ many-to-many với Student thông qua Enrollment

using System.ComponentModel.DataAnnotations;

namespace Lab8_CombinedLoading.Models
{
    /// <summary>
    /// Khóa học - có thể có nhiều học sinh đăng ký (Many-to-Many qua Enrollment)
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

        // Số tín chỉ
        [Display(Name = "Số tín chỉ")]
        public int Credits { get; set; } = 3;

        // Giáo viên phụ trách
        [Display(Name = "Giáo viên")]
        public string? Instructor { get; set; }

        // ========================================
        // Navigation property cho LAZY LOADING (virtual)
        // ========================================
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
