using System.Linq;
using Microsoft.AspNetCore.Http;
using Task = System.Threading.Tasks.Task;
using Newtonsoft.Json;
using Ecommerce_API.Utilities;

namespace Ecommerce_API.Middlewares
{
    public class VersionMiddleware
    {
        private readonly RequestDelegate _next;
        const int MINIMUM_CODE = 1;
        private static string[] WhitelistEndpoints = new[]
        {
            "/api/update",
        };

        public VersionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private Task Allow(HttpContext context)
        {
            return _next.Invoke(context);
        }

        private Task Reject(HttpContext context)
        {
            context.Response.StatusCode = 503;
            return context.Response.WriteAsync(JsonConvert.SerializeObject("Version is less than minimum version"));
        }

        public async Task Invoke(HttpContext context)
        {
            if (WhitelistEndpoints.Any(ep => context.Request.Path.Value.ToLower().Contains(ep)))
            {
                await Allow(context);
                return;
            }

            string vcode = context.Request.Headers["VCode"];
            if (vcode.IsNullOrEmpty())
                vcode = context.Request.Query["vcode"];

            int code = (vcode ?? "").TryParseInt();
            if (code < MINIMUM_CODE)
            {
                await Reject(context);
            }
            else
            {
                await Allow(context);
            }
        }
    }
}