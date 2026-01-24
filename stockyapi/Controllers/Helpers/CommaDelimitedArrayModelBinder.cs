using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace stockyapi.Controllers.Helpers;

[AttributeUsage(AttributeTargets.Parameter)]
public class CommaSeparatedAttribute : ModelBinderAttribute
{
    // When this attribute is used, it tells ASP.NET: 
    // "Use the CommaDelimitedArrayModelBinder for this parameter"
    public CommaSeparatedAttribute() : base(typeof(CommaDelimitedArrayModelBinder))
    {
    }
}

public class CommaDelimitedArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // 1. Get the value provider (where the data is coming from)
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        // 2. Get the actual string value
        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            // This adds the error to the collection instead of throwing
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "The request must be a comma-separated list with at least one item.");
    
            // Tell the binder we failed
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        if (value.Length > 500)
        {
            // This adds the error to the collection instead of throwing
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "The query string is too long");
    
            // Tell the binder we failed
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        // 3. The Logic: Split by comma and trim whitespace
        var items = value.Split([","], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                         .Select(x => x.Trim())
                         .ToArray();

        if (items.Length == 0)
        {
            // This adds the error to the collection instead of throwing
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "The request must be a comma-separated list with at least one item.");
    
            // Tell the binder we failed
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
            
        }

        // 4. Set the result as the "Model"
        bindingContext.Result = ModelBindingResult.Success(items);
        return Task.CompletedTask;
    }
}
