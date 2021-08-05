using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseUrls(ParseAddressBinding(args));
                });

        private static string ParseAddressBinding(string[] args)
        {
            var ipIndex = Array.IndexOf<string>(args, "--ip");
            var portIndex = Array.IndexOf<string>(args, "--port");
            var ip = ipIndex != -1 ? args[ipIndex + 1] : "localhost";
            var port = portIndex != -1 ? args[portIndex + 1] : "5000";
            var url = $"http://{ip}:{port}";
            return url;
        }
    }
}
