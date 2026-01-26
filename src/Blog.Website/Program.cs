using Blog.Website.Data;
using Blog.Website.Middleware;
using ColorCode.Styling;
using Markdig;
using Markdown.ColorCode;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;

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
app.UseRewriter(new RewriteOptions()
    .AddRedirectToNonWwwPermanent()
    .Add(new LowercaseRule()));

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

class LowercaseRule : IRule
{
    public void ApplyRule(RewriteContext context)
    {
        var request = context.HttpContext.Request;
        var path = request.Path;
        var pathAndQuery = request.Path + request.QueryString;

        if (path.HasValue && path.Value.Any(char.IsUpper))
        {
            var newUrl = path.Value.ToLowerInvariant() + request.QueryString;
            var response = context.HttpContext.Response;

            response.StatusCode = StatusCodes.Status301MovedPermanently;
            response.Headers[HeaderNames.Location] = newUrl;

            context.Result = RuleResult.EndResponse;
        }
        else
        {
            context.Result = RuleResult.ContinueRules;
        }
    }
}