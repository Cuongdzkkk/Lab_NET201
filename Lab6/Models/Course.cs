using System.ComponentModel.DataAnnotations;

namespace Lab6.Models;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
    [StringLength(150, ErrorMessage = "Tên khóa học không được vượt quá 150 ký tự")]
    [Display(Name = "Tên khóa học")]
    public string CourseName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã khóa học là bắt buộc")]
    [RegularExpression(@"^[A-Z]{3}[0-9]{3}$", ErrorMessage = "Mã khóa học phải có định dạng 3 chữ cái HOA + 3 chữ số (VD: CSC101)")]
    [Display(Name = "Mã khóa học")]
    public string CourseCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số tín chỉ là bắt buộc")]
    [Range(1, 6, ErrorMessage = "Số tín chỉ phải từ 1 đến 6")]
    [Display(Name = "Số tín chỉ")]
    public int Credits { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày bắt đầu")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày kết thúc")]
    [DateGreaterThan("StartDate", ErrorMessage = "Ngày kết thúc phải sau ngày bắt đầu")]
    public DateTime EndDate { get; set; }

    // Navigation property
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}

// Custom validation attribute for date comparison
public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as DateTime?;

        var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (property == null)
            throw new ArgumentException($"Property {_comparisonProperty} not found");

        var comparisonValue = property.GetValue(validationContext.ObjectInstance) as DateTime?;

        if (currentValue.HasValue && comparisonValue.HasValue)
        {
            if (currentValue <= comparisonValue)
            {
                return new ValidationResult(ErrorMessage ?? $"Ngày phải sau {_comparisonProperty}");
            }
        }

        return ValidationResult.Success;
    }
}
