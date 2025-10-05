using Blog.Website.Data;
using Blog.Website.Middleware;
using ColorCode.Styling;
using Markdig;
using Markdown.ColorCode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddResponseCaching();
builder.Services.AddResponseCompression();
builder.Services.AddSingleton(x => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode(HtmlFormatterType.Css, StyleDictionary.DefaultDark).Build());
builder.Services.AddSingleton<IPostRepository, PostRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    HttpContextExtensions.AlwaysUseHttp = true;
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


using (var rules = File.OpenText(Path.Combine(app.Environment.ContentRootPath, "iis-rewrite-rules.xml")))
{
    var options = new RewriteOptions()
        .AddIISUrlRewrite(rules);
    app.UseRewriter(options);
}

var contentTypeMappings = new FileExtensionContentTypeProvider();
contentTypeMappings.Mappings[".css"] = "text/css; charset=utf-8";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeMappings,
    OnPrepareResponse = ctx =>
    {
        if (!app.Environment.IsDevelopment())
        {
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + TimeSpan.FromDays(365).TotalSeconds;
        }
    }
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Content-Security-Policy", 
        "block-all-mixed-content;" +
        "style-src 'self' 'unsafe-inline';" +
        "font-src 'self';" +
        "form-action 'self';" +
        "frame-ancestors 'self';" +
        "img-src 'self' data:;" +
        "script-src 'self';");
    await next();
});


app.UseResponseCaching();

if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}

app.UseMiddleware<RobotsTxtMiddleware>();
app.UseMiddleware<SitemapMiddleware>();
app.UseMiddleware<RssMiddleware>();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();