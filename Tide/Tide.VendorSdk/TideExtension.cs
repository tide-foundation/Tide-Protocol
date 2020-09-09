using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Controllers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TideExtension
    {
        public static IMvcBuilder AddTideEndpoint(this IServiceCollection services, VendorConfig config)
        {
            services.AddSingleton(config);

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = config.GetSessionKey(),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});

            var assembly = typeof(VendorController).Assembly;
            return services.AddControllers().AddApplicationPart(assembly);
        }
    }
}
