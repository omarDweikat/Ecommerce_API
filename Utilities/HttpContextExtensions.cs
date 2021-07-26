using System.IO;
using System.Threading.Tasks;
using Ecommerce_API.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce_API.Utilities
{
    public static class HttpContextExtensions
    {
        public static User CurrentUser(this IHttpContextAccessor context)
        {
            return context.HttpContext.Items["CurrentUser"] as User;
        }

        public static User CurrentUser(this HttpContext context)
        {
            return context.Items["CurrentUser"] as User;
        }

        public static User CurrentUser(this ActionExecutingContext context)
        {
            return context.HttpContext.Items["CurrentUser"] as User;
        }

        public static byte[] ToBytes(this IFormFile file)
        {
            byte[] contentBytes = null;
            using (var contentStream = new MemoryStream())
            {
                file.CopyTo(contentStream);
                contentBytes = contentStream.ToArray();
            }
            return contentBytes;
        }
    }
}