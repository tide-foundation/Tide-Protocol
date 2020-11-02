using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;

namespace Tide.Ork.Classes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ThrottleAttribute : Attribute, IAsyncActionFilter
    {
        public string Param { get; }
        
        public ThrottleAttribute(string param)
        {
            Param = param;
        }
        
        Task IAsyncActionFilter.OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (TryGetData(context, Param, out var param))
            {
                var blackList = context.HttpContext.RequestServices.GetService<BlackList>();
                if (blackList == null)
                    return next();

                if (blackList.Exist(param)) {
                    context.Result = new StatusCodeResult(429); //Too Many Requests
                    return Task.CompletedTask;
                }

                var throttling = context.HttpContext.RequestServices.GetService<ThrottlingManager>();
                if (throttling == null)
                    return next();

                if (throttling.Throttle(param)) {
                    blackList.Add(param);
                    throttling.Remove(param);
                    context.Result = new StatusCodeResult(429); //Too Many Requests
                    return Task.CompletedTask;
                }
            }

            return next();
        }

        private bool TryGetData(ActionExecutingContext context, string key, out string data) {
            if (context.HttpContext.Request.Query.TryGetValue(key, out var query)) {
                data = query;
                return true;
            }
            else if (context.RouteData.Values.TryGetValue(Param, out var route)) {
                data = route.ToString();
                return true;
            }

            data = string.Empty;
            return false;
        }
    }
}