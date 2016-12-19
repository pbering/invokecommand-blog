using Blog.Data.Lightcore;
using Blog.Website.Middleware;
using Lightcore.Configuration;
using Lightcore.Hosting;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Kernel.Pipelines.Request.Processors;
using Lightcore.Kernel.Pipelines.Response.Processors;
using Lightcore.Kernel.Url;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blog.Website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLightcore(Configuration, pipelines =>
            {
                pipelines.Request.Replace<ResolveLanguageProcessor, ResolveSingleLanguageProcessor>();
                pipelines.Response.Replace<SetHttpCacheHeadersProcessor, SetPublicHttpCacheHeadersProcessor>();
            });

            services.Configure<LightcoreOptions>(options =>
            {
                options.StartItem = "/home";
                options.UseHtmlCache = false;
            });

            services.AddSingleton<IItemStore, FileItemStore>();
            services.AddSingleton<IItemUrlService, NoLanguageItemUrlService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // TODO: Add response cache package?

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMiddleware<RobotsTxtMiddleware>();
            app.UseMiddleware<SitemapMiddleware>();
            app.UseMiddleware<RssMiddleware>();

            app.UseLightcore();
        }
    }
}