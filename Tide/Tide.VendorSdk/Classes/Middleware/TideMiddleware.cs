using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Newtonsoft.Json;
using Tide.Core;

namespace Tide.VendorSdk.Classes.Middleware {
    public class TideMiddleware {
        private static readonly Dictionary<string, RouteTemplate> Templates = new Dictionary<string, RouteTemplate> {
            {"/getusernodes", TemplateParser.Parse("/getusernodes/{username}")},
            {"/createuser", TemplateParser.Parse("/createuser/{username}")},
            {"/confirmuser", TemplateParser.Parse("/confirmuser/{username}")},
            {"/rollbackuser", TemplateParser.Parse("/rollbackuser/{username}")}
        };

        private readonly RequestDelegate _next;
        private readonly TideVendor _tide;

        public TideMiddleware(RequestDelegate next, TideVendor tide) {
            _next = next;
            _tide = tide ?? throw new ArgumentNullException(nameof(tide));
        }

        public async Task Invoke(HttpContext context) {
            var template = Templates.FirstOrDefault(t => context.Request.Path.StartsWithSegments(t.Key));
            if (template.Value == null) {
                await _next.Invoke(context);
                return;
            }

            var matcher = new TemplateMatcher(template.Value, null);
            var values = new RouteValueDictionary();

            matcher.TryMatch(context.Request.Path.Value, values);

            var username = GetRouteValue(values, "username");
            TideResponse response = null;
            switch (template.Key) {
                case "/getusernodes":
                    response = _tide.GetUserNodes(username);
                    break;
                case "/createuser":
                    response = _tide.CreateUser(username, await GetBody<List<string>>(context));
                    break;
                case "/confirmuser":
                    response = _tide.ConfirmUser(username);
                    break;
                case "/rollbackuser":
                    response = _tide.RollbackUser(username);
                    break;
            }

            if (response != null) {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response), Encoding.UTF8);
                return;
            }

            await _next.Invoke(context);
        }

        private string GetRouteValue(RouteValueDictionary route, string key) {
            if (route.TryGetValue(key, out var value)) return value.ToString();
            return null;
        }

        private async Task<T> GetBody<T>(HttpContext context) {
            using (StreamReader stream = new StreamReader(context.Request.Body))
            {
                var body = await stream.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(body);
            }
        }

        private byte[] EncodeResponse(object data) {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
        }

       
    }
}