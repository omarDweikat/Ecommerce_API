using System.Linq;
using Microsoft.AspNetCore.Http;
using Ecommerce_API.Database;
using Ecommerce_API.Models.Users;
using Task = System.Threading.Tasks.Task;
using System.Threading;
using Newtonsoft.Json;
using Ecommerce_API.Utilities;

namespace Ecommerce_API.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private IDatabase _db;

        private static string[] WhitelistEndpoints = new[]
        {
            "/api/auth/",
        };

        public AuthenticationMiddleware(RequestDelegate next, IDatabase db)
        {
            _next = next;
            _db = db;
        }

        private Task Allow(HttpContext context)
        {
            return _next.Invoke(context);
        }

        private Task Reject(HttpContext context)
        {
            context.Response.StatusCode = 401;
            return context.Response.WriteAsync(JsonConvert.SerializeObject("Not Authenticated"));
        }

        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Token"];
            string userId = context.Request.Headers["userId"];
            if (token.IsNullOrEmpty())
                token = context.Request.Query["token"];

            User user = null;
            if (!token.IsNullOrEmpty())
            {

                var validationHandler = new Features.Authentication.TokenValidation.QueryHandler(_db);
                user = await validationHandler.Handle(new Features.Authentication.TokenValidation.Query { Token = token }, CancellationToken.None);

                context.Items["CurrentUser"] = user;
            }

            if (WhitelistEndpoints.Any(ep => context.Request.Path.Value.ToLower().Contains(ep)))
            {
                await Allow(context);
                return;
            }

            if (user != null)
            {
                await Allow(context);
            }
            else
            {
                await Reject(context);
                return;
            }
        }
    }
}