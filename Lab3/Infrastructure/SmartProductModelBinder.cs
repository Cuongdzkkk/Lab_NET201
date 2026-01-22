using Lab3.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Lab3.Infrastructure;

public class SmartProductModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null) throw new ArgumentNullException(nameof(bindingContext));

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);
        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value)) return Task.CompletedTask;

        // Logic: Name-Category-Price (Example: "SonyA7-Camera-3000")
        var parts = value.Split('-');
        if (parts.Length >= 3)
        {
            try
            {
                var product = new Product
                {
                    Name = parts[0].Trim(),
                    Category = parts[1].Trim(),
                    UnitPrice = decimal.Parse(parts[2].Trim()),
                    Color = "Standard",
                    AvailableQuantity = 1,
                    CreatedDate = DateTime.Now,
                    ImageUrl = "https://placehold.co/600x600?text=Fast+Track"
                };
                bindingContext.Result = ModelBindingResult.Success(product);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Parsing Error: {ex.Message}");
            }
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Format Invalid. Use: Name-Category-Price");
        }

        return Task.CompletedTask;
    }
}
