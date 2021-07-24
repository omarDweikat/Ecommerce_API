using System.Linq;
using Microsoft.AspNetCore.Http;
using api.Database;
using api.Models;
using Task = System.Threading.Tasks.Task;
using System.Threading;
using Newtonsoft.Json;
using api.Utilities;

namespace api.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private IDatabase _db;

        private static string[] WhitelistEndpoints = new[]
        {
            "/api/auth/",
            "/api/task/sharedrecord",
            "/api/keytransfer",
            "/api/p",
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
            string token = context.Request.Headers["Authorization"];
            string deviceToken = context.Request.Headers["DeviceId"];
            string userId = context.Request.Headers["userId"];
            string username = context.Request.Headers["username"];
            string department = context.Request.Headers["department"];
            string mobile = context.Request.Headers["mobile"];
            string email = context.Request.Headers["email"];
            string wakeel = context.Request.Headers["wakeel"];
            if (token.IsNullOrEmpty())
                token = context.Request.Query["token"];

            User user = null;
            if (!token.IsNullOrEmpty())
            {

                if (token == api.Features.Constants.General.CRM_TOKEN)
                {
                    user = new User
                    {
                        Id = userId.IsNullOrEmpty() ? -1 : userId.TryParseInt(),
                        Email = email.IsNullOrEmpty() ? "support@iscosoft.com" : email,
                        Name = username.IsNullOrEmpty() ? "Techincal Support" : System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(username)),
                        Department = department.IsNullOrEmpty() ? 1 : department.TryParseInt(),
                        Mobile = mobile.IsNullOrEmpty() ? "970562666783" : mobile,
                        Token = api.Features.Constants.General.CRM_TOKEN,
                        Type = 1,
                        Wakeel = wakeel.IsNullOrEmpty() ? 0 : wakeel.TryParseInt(),
                    };
                    context.Items["CurrentUser"] = user;
                }
                else
                {
                    var validationHandler = new Features.Authentication.TokenValidation.QueryHandler(_crm);
                    user = await validationHandler.Handle(new Features.Authentication.TokenValidation.Query { Token = token, DeviceToken = deviceToken }, CancellationToken.None);

                    context.Items["CurrentUser"] = user;
                }
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