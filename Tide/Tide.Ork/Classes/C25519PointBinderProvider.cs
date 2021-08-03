using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using Tide.Encryption.Ecc;

namespace Tide.Ork.Classes
{
    public class C25519PointBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                return null;
                //throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(C25519Point))
                return new BinderTypeModelBinder(typeof(C25519PointBinder));

            return null;
        }
    }
}