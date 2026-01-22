using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab6.Models;

public class Enrollment
{
    [Key]
    public int EnrollmentId { get; set; }

    [Required(ErrorMessage = "Sinh viên là bắt buộc")]
    [Display(Name = "Sinh viên")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Khóa học là bắt buộc")]
    [Display(Name = "Khóa học")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Ngày đăng ký là bắt buộc")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày đăng ký")]
    public DateTime EnrollmentDate { get; set; }

    [RegularExpression(@"^[ABCDF]$", ErrorMessage = "Điểm chỉ được nhập A, B, C, D hoặc F")]
    [Display(Name = "Điểm")]
    public string? Grade { get; set; }

    // Navigation properties
    [ForeignKey("StudentId")]
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Student? Student { get; set; }

    [ForeignKey("CourseId")]
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public Course? Course { get; set; }
}
