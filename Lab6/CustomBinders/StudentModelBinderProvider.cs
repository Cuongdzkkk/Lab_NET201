using Microsoft.AspNetCore.Mvc.ModelBinding;
using Lab6.Models;

namespace Lab6.CustomBinders;

public class StudentModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(Student))
        {
            return new StudentModelBinder();
        }

        return null;
    }
}
