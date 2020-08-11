using System;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Controllers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TideExtension
    {
        public static IMvcBuilder AddTideEndpoint(this IServiceCollection services, VendorConfig config)
        {
            services.AddSingleton(config);

            var assembly = typeof(VendorController).Assembly;
            return services.AddControllers().AddApplicationPart(assembly);
        }
    }
}
