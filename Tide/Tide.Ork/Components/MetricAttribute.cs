﻿using System;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Tide.Ork.Models;

namespace Tide.Ork.Classes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MetricAttribute : TypeFilterAttribute
    {        
        public MetricAttribute(string name, bool recordSuccess = false) : base(typeof(MetricImplAttribute))
        {
            Arguments = new object[] { name, recordSuccess };
        }

        private class MetricImplAttribute : Attribute, IAsyncResourceFilter
        {
            private readonly TimerOptions _timer;
            private readonly MeterOptions _error;
            private readonly MeterOptions _success;
            private readonly bool recordSuccess;

            public MetricImplAttribute(Settings settings, string name, bool recordSuccess)
            {
                var metrics = new MetricTags(new[] { "node", "flow" }, new[] { settings.Instance.Username, "auth" });

                _timer = new TimerOptions
                {
                    Name = $"{name}_elapsed",
                    MeasurementUnit = Unit.Requests,
                    DurationUnit = TimeUnit.Milliseconds,
                    RateUnit = TimeUnit.Milliseconds,
                    Tags = metrics
                };

                _error = new MeterOptions
                {
                    Name = $"{name}_errors",
                    MeasurementUnit = Unit.Errors,
                    Tags = metrics
                };

                _success = !(this.recordSuccess = recordSuccess) ? null
                    : new MeterOptions {
                        Name = $"{name}_success",
                        MeasurementUnit = Unit.Events,
                        Tags = metrics
                    };
            }

            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
            {
                var metrics = context.HttpContext.RequestServices.GetService<IMetrics>();
                if (metrics is null)
                {
                    await next();
                    return;
                }

                using(metrics.Measure.Timer.Time(_timer))
                {
                    await next();
                    if (context.HttpContext.Response.StatusCode < 200 || context.HttpContext.Response.StatusCode > 299) {
                        metrics.Measure.Meter.Mark(_error, new MetricTags("tmp", "login"));
                    }
                    else if (this.recordSuccess) {
                        metrics.Measure.Meter.Mark(_success, new MetricTags("tmp",  "login"));
                    }
                }
            }
        }
    }
}