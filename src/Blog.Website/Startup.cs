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

            using (var rules = File.OpenText(Path.Combine(env.ContentRootPath, "iis-rewrite-rules.xml")))
            {
                app.UseRewriter(new RewriteOptions().AddIISUrlRewrite(rules));
            }
            
            app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());
 
            app.UseCsp(opts => opts
                               .BlockAllMixedContent()
                               .StyleSources(s => s.Self())
                               .StyleSources(s => s.UnsafeInline())
                               .FontSources(s => s.Self())
                               .FormActions(s => s.Self())
                               .FrameAncestors(s => s.Self())
                               .ImageSources(s => s.Self())
                               .ScriptSources(s => s.Self())
                      );

            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseMiddleware<RobotsTxtMiddleware>();
            app.UseMiddleware<SitemapMiddleware>();
            app.UseMiddleware<RssMiddleware>();
            app.UseMvcWithDefaultRoute();
        }
    }
}