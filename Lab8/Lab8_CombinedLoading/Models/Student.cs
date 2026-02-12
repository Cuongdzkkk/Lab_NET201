// Models/Student.cs
// Entity học sinh trong hệ thống quản lý nâng cao
// Quan hệ many-to-many với Course thông qua Enrollment

using System.ComponentModel.DataAnnotations;

namespace Lab8_CombinedLoading.Models
{
    /// <summary>
    /// Học sinh - có thể đăng ký nhiều khóa học (Many-to-Many qua Enrollment)
    /// Sử dụng virtual cho Lazy Loading
    /// </summary>
    public class Student
    {
        // Khóa chính
        [Key]
        public int StudentId { get; set; }

        // Tên học sinh
        [Required(ErrorMessage = "Tên học sinh là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
        [Display(Name = "Tên học sinh")]
        public string Name { get; set; } = string.Empty;

        // Email học sinh
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        // Lớp
        [Display(Name = "Lớp")]
        public string? Class { get; set; }

        // ========================================
        // Navigation property cho LAZY LOADING (virtual)
        // ========================================
        // Enrollments là junction table - dùng virtual để Lazy Load
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
