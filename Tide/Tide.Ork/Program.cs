using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.Extensions.Configuration;
using Tide.Ork.Models;
using System.Linq;
using Tide.Ork.Repo;
using Tide.Core;

namespace Tide.Ork {
    public class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            var builder =  Host.CreateDefaultBuilder(args);

            var ft = Environment.GetEnvironmentVariable("Settings__Features__Metrics");
            if (bool.TryParse(ft, out var metrics)  && metrics)
            {
                builder.UseMetricsWebTracking().UseMetrics( opt => opt.EndpointOptions = endpoint => {
                    endpoint.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                    endpoint.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
                    endpoint.EnvironmentInfoEndpointEnabled = false;
                });  
            }

            builder.ConfigureServices((hostContext, services) =>
            {
                var settings = hostContext.Configuration.GetSection("Settings").Get<Settings>();
                
                if(args.Any(arg => arg == "--register") && args.Length == 2) {
                    var client = new SimulatorOrkManager(settings.Instance.Username, settings.BuildClient());
                    client.Add(new OrkNode {
                        Id = settings.Instance.Username,
                        Url = args[1],
                        PubKey = settings.Instance.GetPrivateKey().GetPublic().ToString()
                    }).GetAwaiter().GetResult();
                }
            });

            return builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}