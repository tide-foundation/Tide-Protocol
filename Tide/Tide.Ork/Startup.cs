using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tide.Core;
using Tide.Ork.Classes;
using Tide.Ork.Models;
using Tide.Ork.Repo;

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
            services.AddMemoryCache();
            services.AddTransient<IEmailClient, MailKitClient>();
            services.AddTransient<IKeyManagerFactory, SimulatorFactory>();
            services.AddTransient<OrkConfig>();

            var privateKey =  settings.Instance.GetPrivateKey();
            var publicKey = privateKey.GetPublic().ToString();
            var client = new SimulatorClient("https://tidesimulator.azurewebsites.net", "Ork-0",privateKey);

            // Post test
            var guid = Guid.NewGuid().ToString();
            var postRes = client.Post("MattContract", "MattTable", "MattScope", guid, new TestObject() {Field1 = "gioodbye", Field2 = "Hello test"}).Result;

           var deleteRes = client.Delete("MattContract", "MattTable", "MattScope", guid).Result;
        }

        public class TestObject {
            public string Field1 { get; set; }
            public string Field2 { get; set; }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            //else
            //    app.UseHsts();

            app.UseDeveloperExceptionPage(); // TODO: Remove for production

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseCors(builder => builder
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowAnyHeader());

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}