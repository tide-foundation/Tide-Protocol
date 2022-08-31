using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Tide.Core;
using Tide.Simulator.Classes;
using Tide.Simulator.Models;

namespace Tide.Simulator {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            

            var settings = new Settings();
            Configuration.Bind("Settings", settings);
            services.AddSingleton(settings);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settings.BearerKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddDbContext<BlockchainContext>(options => { options.UseSqlServer(settings.Connection, builder => builder.CommandTimeout(6000)); });

            // services.AddDbContext<AccountContext>(options => options.UseSqlite(settings.AccountConnection));
            // services.AddScoped<IAuthentication, Authentication>();

            // TODO: Ask Jose for help making this a factory implementation
            // services.AddScoped<IBlockLayer, CosmosDbService>();
            services.AddScoped<IBlockLayer, SqlBlockLayer>();
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors();



          

        }

   

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime) {

            app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().WithOrigins(new []{"http://localhost:8080", "https://tideexplorer.azurewebsites.net" }).AllowCredentials());

            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
        
            app.UseDeveloperExceptionPage();


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();

            });
        }
    }
}