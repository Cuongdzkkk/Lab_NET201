// Models/Enrollment.cs
// Entity Junction table cho quan hệ Many-to-Many giữa Student và Course
// Đây là bảng trung gian chứa khóa ngoại của cả 2 bảng

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8_CombinedLoading.Models
{
    /// <summary>
    /// Bảng đăng ký - Junction table cho quan hệ Many-to-Many
    /// Liên kết Student với Course
    /// </summary>
    public class Enrollment
    {
        // Khóa chính của Enrollment
        [Key]
        public int EnrollmentId { get; set; }

        // ========================================
        // KHÓA NGOẠI cho Student
        // ========================================
        [Required]
        [Display(Name = "Học sinh")]
        public int StudentId { get; set; }

        // ========================================
        // KHÓA NGOẠI cho Course
        // ========================================
        [Required]
        [Display(Name = "Khóa học")]
        public int CourseId { get; set; }

        // Ngày đăng ký
        [Display(Name = "Ngày đăng ký")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // Điểm số (nullable vì có thể chưa có điểm)
        [Display(Name = "Điểm")]
        [Column(TypeName = "decimal(3,1)")]
        public decimal? Grade { get; set; }

        // ========================================
        // Navigation properties với virtual cho Lazy Loading
        // ========================================
        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }
}
