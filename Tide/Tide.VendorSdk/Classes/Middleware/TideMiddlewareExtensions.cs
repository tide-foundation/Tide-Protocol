using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Tide.VendorSdk.Configuration;

namespace Tide.VendorSdk.Classes.Middleware {
    public static class TideMiddlewareExtensions {
        private const string V1PathMatch = "/tide/v1";
        private const string V2PathMatch = "/tide/v2";

        public static IServiceCollection AddTide(this IServiceCollection services, string vendorId,
            Action<ITideConfiguration> configuration)
        {
            var configurationInstance = TideConfiguration.Configuration;
            configurationInstance.VendorId = vendorId;
            configuration(configurationInstance);

            return services.AddSingleton(provider => new TideVendor(configurationInstance));
        }

        public static IApplicationBuilder UseTide(this IApplicationBuilder app) {
            var services = app.ApplicationServices;
            var tideVendor = services.GetRequiredService<TideVendor>();

            app.Map(new PathString(V1PathMatch), x => x.UseMiddleware<TideMiddleware>(tideVendor));

            // app.Map(new PathString(V2PathMatch), x => x.UseMiddleware<TideMiddleware>(tideVendor, routes));

            return app;
        }
    }
}