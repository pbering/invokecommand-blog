using System.IO;
using Blog.Website.Data;
using Blog.Website.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Blog.Website
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddResponseCaching();
            services.AddSingleton<IPostRepository, PostRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, @".well-known")),
                RequestPath = new PathString("/.well-known")
            });

            app.UseMiddleware<RobotsTxtMiddleware>();
            app.UseMiddleware<SitemapMiddleware>();
            app.UseMiddleware<RssMiddleware>();

            app.UseMvcWithDefaultRoute();
        }
    }
}