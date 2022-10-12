using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tide.Core;
using Tide.Encryption.Ed;

namespace Tide.Ork.Classes {

    public class Ed25519PointBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var fieldName = context.ModelName;
            var fieldValue = context.ValueProvider.GetValue(fieldName);
            if (fieldValue == ValueProviderResult.None || string.IsNullOrWhiteSpace(fieldValue.FirstValue))
                return Task.CompletedTask;

            if (!Helpers.TryFromBase64String(fieldValue.FirstValue, out var buffer))
            {
                context.ModelState.TryAddModelError(fieldName, $"Is not a valid base64 string.");
                return Task.CompletedTask;
            }

            var model = Ed25519Point.From(buffer);
            if (!model.IsValid())
            {
                context.ModelState.TryAddModelError(fieldName, $"Is not a valid point.");
                return Task.CompletedTask;
            }

            context.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}