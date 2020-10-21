using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Tide.Encryption.AesMAC;
using Tide.Vendor.Classes;
using Tide.Vendor.Models;
using Tide.VendorSdk.Classes;
using VueCliMiddleware;

namespace Tide.Vendor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
        


            var settings = new Settings();
            Configuration.Bind("Settings", settings);
            services.AddSingleton(settings);


            services.AddDbContext<VendorDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    builder => builder.CommandTimeout(6000));
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = settings.Audience,
                        ValidAudience = settings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.BearerKey))
                    };
                });


            if (settings.DevFront) services.AddSpaStaticFiles(opt => opt.RootPath = "Client/dist");
            services.AddSingleton<IAuthentication, Authentication>();
            services.AddSingleton<IVendorRepo, VendorRepo>();
            services.AddTideEndpoint(settings.Keys);
            services.AddControllers();

        }



        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,Settings settings)
        {
            if (env.IsDevelopment() || true)
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
          

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());

        
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

                if (env.IsDevelopment() && settings.DevFront)
                {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "Client" },
                        npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                        regex: "Compiled successfully"
                    );
                }
            });

            if (settings.DevFront) {
                app.UseSpaStaticFiles();
                app.UseSpa(spa => spa.Options.SourcePath = "Client");
            }
        }
    }
}
