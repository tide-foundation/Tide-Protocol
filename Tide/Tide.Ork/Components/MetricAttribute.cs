using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Tide.Ork.Classes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MetricAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly TimerOptions _timer;
        private readonly MeterOptions _error;
        private readonly MeterOptions _success;
        private readonly bool recordSuccess;

        public MetricAttribute(string name, bool recordSuccess = false)
        {
            _timer = new TimerOptions
            {
                Name = $"{name}_elapsed",
                MeasurementUnit = Unit.Requests,
                DurationUnit = TimeUnit.Milliseconds,
                RateUnit = TimeUnit.Milliseconds
            };

            _error = new MeterOptions
            {
                Name = $"{name}_errors",
                MeasurementUnit = Unit.Errors
            };

            _success = !(this.recordSuccess = recordSuccess) ? null
                : new MeterOptions {
                    Name = $"{name}_success",
                    MeasurementUnit = Unit.Events
                };
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var metrics = context.HttpContext.RequestServices.GetService<IMetrics>();
            if (metrics is null) {
                await next();
                return;
            }
            
            using(metrics.Measure.Timer.Time(_timer))
            {
                await next();
                if (context.HttpContext.Response.StatusCode < 200 || context.HttpContext.Response.StatusCode > 299) {
                    metrics.Measure.Meter.Mark(_error);
                }
                else if (this.recordSuccess) {
                    metrics.Measure.Meter.Mark(_success);
                }
            }
        }
    }
}