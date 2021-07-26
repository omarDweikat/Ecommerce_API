using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;
using Ecommerce_API.Middlewares;
using DalSoft.Hosting.BackgroundQueue.DependencyInjection;
using Ecommerce_API.Database;
using MediatR;
using FluentValidation.AspNetCore;
using Ecommerce_API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Ecommerce_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddBackgroundQueue(ex => Serilog.Log.Error(ex.ToString()));
            services.AddMediatR(typeof(Startup));
            services.AddSingleton<IDatabase, Database.Database>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var LoggerConfiguration = new LoggerConfiguration()
                        .MinimumLevel.Information();
#if DEBUG
            LoggerConfiguration.WriteTo.Console();
#else
            LoggerConfiguration.WriteTo.File(Path.Combine("Log","log.txt"), rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true);
#endif
            Log.Logger = LoggerConfiguration.CreateLogger();

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder => { builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetPreflightMaxAge(TimeSpan.FromDays(1)); })
            .UseMiddleware<LoggingMiddleware>()
            .UseMiddleware<AuthenticationMiddleware>()
            .UseMiddleware<VersionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
