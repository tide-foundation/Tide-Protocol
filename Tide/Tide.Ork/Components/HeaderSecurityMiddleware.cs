using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace Tide.Ork.Classes
{
    public class HeaderSecurityMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderSecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<HeaderSecurityMiddleware> logger)
        {
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.TryGetValue("Content-Type", out var content)) {
                    logger.LogError("\"Content-Type\" was not found for {path}", context.Request.Path.Value);
                    return Task.CompletedTask;
                }

                if (!content.Any(cnt => cnt == "text/html")) {
                    return Task.CompletedTask;
                }

                if (!context.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }

                if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }

                //TODO: The 'unsafe-inline' for style-src should be removed and implemented in a better way
                var csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self'; connect-src *; style-src 'unsafe-inline'";
                if (!context.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.Response.Headers.Add("Content-Security-Policy", csp);
                }
                if (!context.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.Response.Headers.Add("X-Content-Security-Policy", csp);
                }

                var referrer_policy = "no-referrer";
                if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.Response.Headers.Add("Referrer-Policy", referrer_policy);
                }
                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}