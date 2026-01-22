using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab2.Models;

namespace Lab2.Infrastructure.Binders
{
    public class ProductStringBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
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

            var parts = value.Split('-');

            if (parts.Length < 3)
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, "Invalid format. Expected: Name-Description-Price");
                return Task.CompletedTask;
            }

            var name = parts[0].Trim();
            var description = parts[1].Trim();
            
            if (!decimal.TryParse(parts[2].Trim(), out var price))
            {
                bindingContext.ModelState.TryAddModelError(
                    bindingContext.ModelName, "Invalid price format.");
                return Task.CompletedTask;
            }

            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price
            };

            bindingContext.Result = ModelBindingResult.Success(product);
            return Task.CompletedTask;
        }
    }
}
