using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Ecommerce_API.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
namespace Ecommerce_API.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var sw = new Stopwatch();
            try
            {
                sw.Start();
                await _next(context);
                sw.Stop();
                LogRequest(context, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                await HandleExceptionAsync(context, ex, sw.ElapsedMilliseconds);
            }
        }

        private void LogRequest(HttpContext context, long ms)
        {
            if (context.Request.Method == "OPTIONS")
                return;
            var currentUser = context.CurrentUser();
            var version = context.Request.Headers["VClient"];
            var user = currentUser == null ? "" : currentUser.ID + "";
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            Serilog.Log.Information($"U{user} {version} {method} {path} {ms}ms");
        }

        private void LogException(HttpContext context, Exception exception, long ms)
        {
            var currentUser = context.CurrentUser();
            var version = context.Request.Headers["VClient"];
            var user = currentUser == null ? "" : currentUser.ID + "";
            var method = context.Request.Method;
            var path = context.Request.Path.ToString();
            Serilog.Log.Error($"U{user} {version} {method} {path} {ms}ms " + exception.Demystify());
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, long ms)
        {
            LogException(context, exception, ms);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;
            return context.Response.WriteAsync(JsonConvert.SerializeObject(exception.Message));
        }
    }
}