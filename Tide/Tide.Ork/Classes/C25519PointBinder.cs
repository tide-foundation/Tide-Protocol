using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tide.Core;
using Tide.Encryption.Ecc;

namespace Tide.Ork.Classes {

    public class C25519PointBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            var value = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;
            
            if (!Helpers.TryFromBase64String(value, out var buffer))
            {
                bindingContext.ModelState.TryAddModelError(modelName, $"{modelName} is not a valid base64 string.");
                return Task.CompletedTask;
            }

            var model = C25519Point.From(buffer);
            if (!model.IsValid)
            {
                bindingContext.ModelState.TryAddModelError(modelName, $"{modelName} is not a valid point.");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}