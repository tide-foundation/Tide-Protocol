using System;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tide.Ork.Classes;
using Tide.Ork.Models;
using Tide.Ork.Models.Serialization;
using Tide.Ork.Repo;
using VueCliMiddleware;

namespace Tide.Ork {
    public class Startup {
        private string _version;
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
            _version = typeof(Program).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers(opt => opt.ModelBinderProviders.Insert(0, new MainBinderProvider()))
                .AddJsonOptions(opt => {
                    opt.JsonSerializerOptions.Converters.Add(new C25519PointConverter());
                    opt.JsonSerializerOptions.Converters.Add(new AesKeyConverter());
                });

            var settings = new Settings();
            Configuration.Bind(nameof(Settings), settings);

            services.AddSingleton(settings);
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddTransient<IEmailClient, SendGridEmailClient>();
            services.AddTransient<OrkConfig>();
            services.AddSignalR();
            services.AddTransient<IVerificationKeyRepo, VerificationKeyRepo>();
            

            services.AddSpaStaticFiles(opt => opt.RootPath = "Enclave/dist");

            if (settings.Features.Metrics)
                services.AddMetrics();
            
            if (settings.Features.Throttling)
                services.ConfigureThrottling();

            if (settings.Features.Memory)
                services.AddTransient<IKeyManagerFactory, MemoryFactory>();
            else
                services.AddTransient<IKeyManagerFactory, SimulatorFactory>();

            services.AddCors();
            services.AddAuthentication("keyAuth").AddScheme<AuthenticationSchemeOptions, VerificationKeyAuthenticationHandler>("keyAuth", null);
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
         
            #if DEBUG
            app.UseDeveloperExceptionPage(); // TODO: Remove for production
            #endif

            if (env.IsProduction())
                app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
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

                if (env.IsDevelopment() && settings.Features.DevFront)
                {
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "Enclave" },
                        npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                        regex: "Compiled successfully"
                    );
                }
            });

            if (settings.Features.CSP)
                app.UseMiddleware<HeaderSecurityMiddleware>();

            if (settings.Features.DevFront)
            {
                app.UseSpaStaticFiles();
                app.UseSpa(spa => spa.Options.SourcePath = "Enclave");
            }
        }
    }
}