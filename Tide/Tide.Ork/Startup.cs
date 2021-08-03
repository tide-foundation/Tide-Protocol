using System;
using System.Collections.Generic;
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
using App.Metrics.AspNetCore;
using Newtonsoft.Json;
using Tide.Encryption.Ecc;

namespace Tide.Ork {
    public class Startup {
        private string _version;
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            _version = typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new C25519PointBinderProvider());
            });

            var settings = new Settings();
            Configuration.Bind("Settings", settings);

            services.AddSingleton(settings);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddTransient<IEmailClient, SendGridEmailClient>();
            services.AddTransient<OrkConfig>();
            services.AddSignalR();

            services.AddSpaStaticFiles(opt => opt.RootPath = "Client/dist");

            if (settings.Features.Metrics)
                services.AddMetrics();
            
            if (settings.Features.Throttling)
                services.ConfigureThrottling();

            if (settings.Features.Memory)
                services.AddTransient<IKeyManagerFactory, MemoryFactory>();
            else
                services.AddTransient<IKeyManagerFactory, SimulatorFactory>();

            services.AddCors();

            var privString = "AOAxMtmYfyI98Tr5jiQ77kZGA3goBctEWnDFTWnSOzol3pIbKWvLkkW83s55zJNczOxcbKXdeRSheFXmlDeQWS+KTCkfERyiI5J1i8Xlwe4clgY10LAfV0Ds9xP4QOhK";

            var priv = C25519Key.Parse(privString);
            var pubString = priv.GetPublic().ToString();
        }

     

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,Settings settings) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().WithOrigins(new[] { "http://localhost:8081", "http://172.26.17.60:8080" }).AllowCredentials());
            app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            //else
            //    app.UseHsts();

            app.UseStaticFiles();
         
            app.UseDeveloperExceptionPage(); // TODO: Remove for production

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseCors(builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());

            app.Use((context, next) =>
            {
                context.Response.Headers["Ork-Id"] = settings.Instance.Username;

                var version = _version;

                context.Response.Headers["User-Agent"] = $"Ork-Version:{version}";
                return next.Invoke();
            });

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapHub<EnclaveHub>("/enclave-hub");

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

            if (settings.DevFront)
            {
                app.UseSpaStaticFiles();
                app.UseSpa(spa => spa.Options.SourcePath = "Client");
            }
        }
    }



}