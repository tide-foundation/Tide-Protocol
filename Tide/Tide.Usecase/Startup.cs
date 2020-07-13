

using System;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            var settings = new Settings();
            Configuration.Bind("Settings", settings);
            services.AddSingleton(settings);


            services.AddDbContext<VendorContext>(options => { options.UseSqlServer(settings.Connection, builder => builder.CommandTimeout(6000)); });

            services.AddScoped<TideVendor>();
            TideVendor.Init("VendorId");

            

            services.AddLiveReload();
            services.AddControllers();
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
 
            // app.Map("/test", HandleTest);
            //app.UseTide();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseLiveReload();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();

          
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

    //public class TideMiddlewareExtensions
    //{
    //    public IRouter BuildRouter(IApplicationBuilder applicationBuilder)
    //    {
    //        var builder = new RouteBuilder(applicationBuilder);

    //        builder.MapMiddlewareGet("/tide/v1/test", appBuilder => {
    //            appBuilder.Use(Middleware);
    //        });

    //        return builder.Build();
    //    }

    //    private RequestDelegate Middleware(RequestDelegate arg) {
    //        throw new System.NotImplementedException();
    //    }
    //}
}
