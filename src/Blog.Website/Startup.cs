﻿using System.IO;
using Blog.Website.Data;
using Blog.Website.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddResponseCaching();
            services.AddSingleton<IPostRepository, PostRepository>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                HttpContextExtensions.AlwaysUseHttp = true;
            }

            using (var rules = File.OpenText(Path.Combine(env.ContentRootPath, "iis-rewrite-rules.xml")))
            {
                app.UseRewriter(new RewriteOptions().AddIISUrlRewrite(rules));
            }

            app.UseHsts(hsts => hsts.AllResponses().MaxAge(365).IncludeSubdomains());
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

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}