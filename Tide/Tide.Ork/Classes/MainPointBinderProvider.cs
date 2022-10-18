using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using Tide.Encryption.Ed;
using Tide.Encryption.AesMAC;

namespace Tide.Ork.Classes
{
    public class MainBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                return null;
                //throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(Ed25519Point))
                return new BinderTypeModelBinder(typeof(Ed25519PointBinder));

            if (context.Metadata.ModelType == typeof(AesKey))
                return new BinderTypeModelBinder(typeof(AesKeyBinder));

            return null;
        }
    }
}