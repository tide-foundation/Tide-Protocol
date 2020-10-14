using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tide.Core;
using Tide.Encryption.AesMAC;
using Tide.Ork.Classes;
using Tide.Ork.Models;
using Tide.Ork.Repo;
using VueCliMiddleware;

namespace Tide.Ork {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
           
            var settings = new Settings();
            Configuration.Bind("Settings", settings);

            services.AddSingleton(settings);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddTransient<IEmailClient, MailKitClient>();
            services.AddTransient<OrkConfig>();

            services.AddSpaStaticFiles(opt => opt.RootPath = "Client/dist");

            if (settings.Memory)
                services.AddTransient<IKeyManagerFactory, MemoryFactory>();
            else
                services.AddTransient<IKeyManagerFactory, SimulatorFactory>();
        }

<<<<<<< Updated upstream
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
=======
>>>>>>> Stashed changes
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,Settings settings) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseDeveloperExceptionPage(); // TODO: Remove for production

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseCors(builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());

<<<<<<< Updated upstream
            app.Use((context, next) =>
            {
                context.Response.Headers["Ork-Id"] = settings.Instance.Username;

                var version = typeof(Program).Assembly
                    .GetCustomAttribute<AssemblyFileVersionAttribute>()
                    ?.Version;

                context.Response.Headers["User-Agent"] = $"Ork-Version:{version}";
                return next.Invoke();
            });

            app.UseEndpoints(endpoints => endpoints.MapControllers());
=======
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

                if (env.IsDevelopment() && settings.DevFront) {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "Client" },
                        npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                        regex: "Compiled successfully"
                    );
                }
            });

            app.UseSpa(spa => spa.Options.SourcePath = "Client");
>>>>>>> Stashed changes
        }
    }



}