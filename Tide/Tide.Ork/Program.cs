using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;

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

            return builder.ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}