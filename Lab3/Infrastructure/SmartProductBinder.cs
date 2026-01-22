using Lab3.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Lab3.Infrastructure
{
    public class SmartProductBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Look for the value provided in the request (e.g., from form field named "product")
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            // Expected format: Name-Category-Price
            // Example: "Leica M11-Camera-9000"
            var parts = value.Split('-');

            if (parts.Length >= 3)
            {
                try
                {
                    var name = parts[0].Trim();
                    var category = parts[1].Trim();
                    if (decimal.TryParse(parts[2].Trim(), out var price))
                    {
                        var product = new Product
                        {
                            Name = name,
                            Category = category,
                            UnitPrice = price,
                            // Default values for fields not in the "spell"
                            Color = "Mist",
                            AvailableQuantity = 1,
                            CreatedDate = DateTime.Now,
                            ImageUrl = "https://placehold.co/400x300?text=Mist+Weaved"
                        };

                        bindingContext.Result = ModelBindingResult.Success(product);
                    }
                    else
                    {
                        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid price format.");
                    }
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Parsing error: {ex.Message}");
                }
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid format. Expected: Name-Category-Price");
            }

            return Task.CompletedTask;
        }
    }
}
