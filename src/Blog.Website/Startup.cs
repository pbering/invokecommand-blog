using Blog.Data;
using Lightcore.Configuration;
using Lightcore.Hosting;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Kernel.Pipelines.Request.Processors;
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
                                      pipelines.Request
                                               .Replace<ResolveLanguageProcessor, ResolveSingleLanguageProcessor>());

            services.Configure<LightcoreOptions>(options =>
            {
                options.StartItem = "/home";
                options.UseHtmlCache = false;
            });

            services.AddSingleton<IItemStore, FileItemStore>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // TODO: Add output cache?

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO: Handle exceptions...
                app.UseExceptionHandler("/Errors/500");
            }

            app.UseStaticFiles();

            // TODO: For rss, sitemap and robots?
            app.Map("/api", builder => { app.UseMvc(); });

            app.UseLightcore();
        }
    }
}