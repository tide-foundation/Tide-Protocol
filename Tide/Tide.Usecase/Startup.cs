using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tide.Usecase.Models;
using Tide.VendorSdk;
using Tide.VendorSdk.Classes;
using Tide.VendorSdk.Classes.Middleware;
using Tide.VendorSdk.Configuration;
using VueCliMiddleware;
using Westwind.AspNetCore.LiveReload;

namespace Tide.Usecase
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {

            var settings = new Settings();
            Configuration.Bind("Settings", settings);

            services.AddSingleton<IVendorRepo, VendorRepo>();
            services.AddTideEndpoint(settings.Keys.CreateVendorConfig());
            services.AddControllers();

            //services.AddDbContext<VendorContext>(options => { options.UseSqlServer(settings.Connection, builder => builder.CommandTimeout(6000)); });

            //services.AddLiveReload();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }

            });
        }
    }
}
