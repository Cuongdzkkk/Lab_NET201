using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab5.Models;

namespace Lab5.Binders
{
    public class CourseModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Try to get custom format value first
            var customValueResult = bindingContext.ValueProvider.GetValue("customCourse");
            
            // If custom format provided, parse it
            if (customValueResult != ValueProviderResult.None && !string.IsNullOrEmpty(customValueResult.FirstValue))
            {
                return BindFromCustomFormat(bindingContext, customValueResult.FirstValue);
            }

            // Otherwise, use default model binding (standard form)
            return BindFromForm(bindingContext);
        }

        private Task BindFromCustomFormat(ModelBindingContext bindingContext, string value)
        {
            // Format: "Name|StartDate"
            // Example: "Math 101|2024-09-01"
            
            var parts = value.Split('|');

            if (parts.Length != 2)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    "Custom format must be 'Name|StartDate'");
                return Task.CompletedTask;
            }

            try
            {
                var course = new Course
                {
                    Name = parts[0],
                    StartDate = DateTime.Parse(parts[1])
                };

                bindingContext.Result = ModelBindingResult.Success(course);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    $"Error parsing custom format: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        private Task BindFromForm(ModelBindingContext bindingContext)
        {
            // Get values from standard form fields
            var nameResult = bindingContext.ValueProvider.GetValue("Name");
            var startDateResult = bindingContext.ValueProvider.GetValue("StartDate");

            if (nameResult == ValueProviderResult.None)
            {
                return Task.CompletedTask; // No form data provided
            }

            try
            {
                var course = new Course
                {
                    Name = nameResult.FirstValue,
                    StartDate = DateTime.Parse(startDateResult.FirstValue ?? DateTime.Now.ToString())
                };

                bindingContext.Result = ModelBindingResult.Success(course);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, 
                    $"Error binding from form: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
