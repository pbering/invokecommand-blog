using System.IO;
using Blog.Website.Data;
using Blog.Website.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
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

            using (var iisRewriteRules = File.OpenText(Path.Combine(env.ContentRootPath, "iis-rewrite-rules.xml")))
            {
                var options = new RewriteOptions().AddIISUrlRewrite(iisRewriteRules);

                app.UseRewriter(options);
            }

            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseMiddleware<RobotsTxtMiddleware>();
            app.UseMiddleware<SitemapMiddleware>();
            app.UseMiddleware<RssMiddleware>();
            app.UseMvcWithDefaultRoute();
        }
    }
}