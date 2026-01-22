using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab6.Models;
using System.Globalization;

namespace Lab6.CustomBinders;

public class StudentModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;

        // Try to get the custom data format from form
        var customDataProvider = bindingContext.ValueProvider.GetValue("customData");
        
        if (customDataProvider != ValueProviderResult.None && !string.IsNullOrWhiteSpace(customDataProvider.FirstValue))
        {
            var customData = customDataProvider.FirstValue;
            
            try
            {
                var student = ParseCustomFormat(customData);
                bindingContext.Result = ModelBindingResult.Success(student);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(modelName, $"Lỗi phân tích dữ liệu: {ex.Message}");
                bindingContext.Result = ModelBindingResult.Failed();
            }
            
            return Task.CompletedTask;
        }

        // If no custom data, fall back to default binding (regular form fields)
        // This allows the standard form to work normally
        return Task.CompletedTask;
    }

    private Student ParseCustomFormat(string customData)
    {
        // Format: "Name:value|Email:value|Phone:value|DOB:yyyy-MM-dd|Address:value|GPA:value"
        // Example: "Name:Nguyen Van A|Email:a@email.com|Phone:0901234567|DOB:2000-01-15|Address:HCM|GPA:3.5"

        var student = new Student();
        var parts = customData.Split('|');

        foreach (var part in parts)
        {
            var keyValue = part.Split(':', 2);
            if (keyValue.Length != 2)
            {
                throw new FormatException($"Định dạng không hợp lệ: {part}. Phải có dạng Key:Value");
            }

            var key = keyValue[0].Trim();
            var value = keyValue[1].Trim();

            switch (key.ToLower())
            {
                case "name":
                    if (string.IsNullOrWhiteSpace(value) || value.Length < 3 || value.Length > 100)
                    {
                        throw new FormatException("Tên phải từ 3 đến 100 ký tự");
                    }
                    student.StudentName = value;
                    break;

                case "email":
                    if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
                    {
                        throw new FormatException("Email không hợp lệ");
                    }
                    student.Email = value;
                    break;

                case "phone":
                    if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^0[0-9]{9}$"))
                    {
                        throw new FormatException("Số điện thoại phải có định dạng 0XXXXXXXXX (10 chữ số)");
                    }
                    student.PhoneNumber = value;
                    break;

                case "dob":
                    if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dob))
                    {
                        throw new FormatException("Ngày sinh phải có định dạng yyyy-MM-dd");
                    }
                    var age = DateTime.Today.Year - dob.Year;
                    if (dob.Date > DateTime.Today.AddYears(-age)) age--;
                    if (age < 18)
                    {
                        throw new FormatException("Sinh viên phải từ 18 tuổi trở lên");
                    }
                    student.DateOfBirth = dob;
                    break;

                case "address":
                    if (value.Length > 200)
                    {
                        throw new FormatException("Địa chỉ không được vượt quá 200 ký tự");
                    }
                    student.Address = value;
                    break;

                case "gpa":
                    if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal gpa))
                    {
                        throw new FormatException("GPA phải là số");
                    }
                    if (gpa < 0 || gpa > 4.0m)
                    {
                        throw new FormatException("GPA phải từ 0.0 đến 4.0");
                    }
                    student.GPA = gpa;
                    break;

                default:
                    throw new FormatException($"Trường không hợp lệ: {key}");
            }
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(student.StudentName))
        {
            throw new FormatException("Thiếu trường bắt buộc: Name");
        }
        if (string.IsNullOrWhiteSpace(student.Email))
        {
            throw new FormatException("Thiếu trường bắt buộc: Email");
        }
        if (string.IsNullOrWhiteSpace(student.PhoneNumber))
        {
            throw new FormatException("Thiếu trường bắt buộc: Phone");
        }
        if (student.DateOfBirth == DateTime.MinValue)
        {
            throw new FormatException("Thiếu trường bắt buộc: DOB");
        }
        if (student.GPA == 0)
        {
            throw new FormatException("Thiếu trường bắt buộc: GPA");
        }

        return student;
    }
}
