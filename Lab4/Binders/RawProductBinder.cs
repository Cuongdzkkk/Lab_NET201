using System.Net.Mime;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class RawProductBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        var value = context.ValueProvider.GetValue(context.ModelName).FirstValue;
        context.Result = ModelBindingResult.Success(value);
        return Task.CompletedTask;
    }
}