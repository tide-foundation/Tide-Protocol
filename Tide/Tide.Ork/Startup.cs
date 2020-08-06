using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tide.Ork.Classes;
using Tide.Ork.Models;

namespace Tide.Ork {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            var settings = new Settings();
            Configuration.Bind("Settings", settings);

            services.AddSingleton(settings);
            services.AddHttpContextAccessor();
            services.AddTransient<IEmailClient, MailKitClient>();
            services.AddTransient<IKeyManagerFactory, MemoryFactory>();

            // TODO: GET JOSE TO HELP INITIALIZING THIS INSIDE FACTORY
            services.AddSingleton(new SimulatorClient(settings.Endpoints.Simulator.Api, settings.Instance.Username, settings.Endpoints.Simulator.Password));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseDeveloperExceptionPage(); // TODO: Remove for production

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader());

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}