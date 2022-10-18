using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tide.Core;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes {

    public class AesKeyBinder : IModelBinder
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

            context.Result = ModelBindingResult.Success(AesKey.Parse(buffer));
            return Task.CompletedTask;
        }
    }
}