using System.ComponentModel.DataAnnotations;

namespace Lab6.Models;

public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Tên sinh viên là bắt buộc")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên sinh viên phải từ 3 đến 100 ký tự")]
    [Display(Name = "Tên sinh viên")]
    public string StudentName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải có định dạng 0XXXXXXXXX (10 chữ số)")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    [AgeValidation(18, ErrorMessage = "Sinh viên phải từ 18 tuổi trở lên")]
    public DateTime DateOfBirth { get; set; }

    [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "GPA là bắt buộc")]
    [Range(0.0, 4.0, ErrorMessage = "GPA phải từ 0.0 đến 4.0")]
    [Display(Name = "GPA")]
    public decimal GPA { get; set; }

    // Navigation property
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

// Custom validation attribute for age
public class AgeValidationAttribute : ValidationAttribute
{
    private readonly int _minimumAge;

    public AgeValidationAttribute(int minimumAge)
    {
        _minimumAge = minimumAge;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime dateOfBirth)
        {
            var age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < _minimumAge)
            {
                return new ValidationResult(ErrorMessage ?? $"Tuổi phải từ {_minimumAge} trở lên");
            }
        }

        return ValidationResult.Success;
    }
}
