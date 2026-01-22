using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab5.Models;

namespace Lab5.Binders
{
    public class QuickStudentBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue("student");

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue("student", valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Format: FirstName|LastName|Email|DateBirth
            var parts = value.Split('|');

            if (parts.Length != 4)
            {
                bindingContext.ModelState.TryAddModelError(
                    "student", "Format phải là 'FirstName|LastName|Email|DateBirth'");
                return Task.CompletedTask;
            }

            try
            {
                var student = new Student
                {
                    FirstName = parts[0].Trim(),
                    LastName = parts[1].Trim(),
                    StudentDetails = new StudentDetails
                    {
                        Email = parts[2].Trim(),
                        DateBirth = DateTime.Parse(parts[3].Trim())
                    }
                };

                bindingContext.Result = ModelBindingResult.Success(student);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    "student", $"Lỗi parse: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
