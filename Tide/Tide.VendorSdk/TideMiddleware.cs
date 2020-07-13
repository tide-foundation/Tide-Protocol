using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Tide.VendorSdk {
    public class TideMiddleware {
        private readonly RequestDelegate _next;

        public TideMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, TideVendor vendor) {
            try {
                var p = httpContext.Request.Path.HasValue && httpContext.Request.Path.Value.StartsWith("/tide")
                    ? httpContext.Request.Path.Value.Substring(9)
                    : null;

                if (!string.IsNullOrEmpty(p)) {
                    if (p.StartsWith("test1")) {
                      
                    }

                    if (p.StartsWith("test2")) {
                     
                    }

                    if (p.StartsWith("{username}")) {
                     
                    }
                }
            }
            catch (Exception) {
                // Unhandled tide path error
            }

            await _next.Invoke(httpContext);
        }
    }


    public static class TideMiddlewareExtensions {
        public static IApplicationBuilder UseTide(this IApplicationBuilder app) {
            var tide = 1;


            //app.Map("/tide/v1/", builder => {
            //    builder.
            //})

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");

            });
            //app.Use()
            // app.Map("/tide/v1/GetUserNodes/{username}", HandleTest);
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapVersion("/version");
            //    endpoints.MapDefaultControllerRoute();
            //});
            return app.UseMiddleware<TideMiddleware>();
        }

        //private static void HandleTest(IApplicationBuilder app) {
        //    app.Run(async context => { await context.Response.WriteAsync("Hello from test."); });
        //}

        //public static IEndpointRouteBuilder UseTide(this IEndpointRouteBuilder endpoints) {

        //}

    }
}