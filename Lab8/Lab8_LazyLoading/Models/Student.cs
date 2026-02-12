// Models/Student.cs
// Entity đại diện cho học sinh trong hệ thống quản lý học sinh
// QUAN TRỌNG: Sử dụng từ khóa 'virtual' cho navigation properties để bật Lazy Loading

using System.ComponentModel.DataAnnotations;

namespace Lab8_LazyLoading.Models
{
    /// <summary>
    /// Học sinh - có thể đăng ký nhiều khóa học
    /// LƯU Ý: Navigation property phải là 'virtual' để Lazy Loading hoạt động
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

        // Ngày sinh
        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // ========================================
        // LAZY LOADING: Navigation property với 'virtual'
        // ========================================
        // Từ khóa 'virtual' cho phép EF Core tạo proxy class
        // Override property này để tự động load dữ liệu khi truy cập
        // KHÔNG CẦN dùng .Include() - dữ liệu sẽ tự động load
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
