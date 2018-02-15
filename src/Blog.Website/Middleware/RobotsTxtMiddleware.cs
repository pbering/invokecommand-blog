using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Blog.Website.Middleware
{
    public class RobotsTxtMiddleware
    {
        private readonly RequestDelegate _next;

        public RobotsTxtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(new PathString("/robots.txt"), StringComparison.OrdinalIgnoreCase))
            {
                var text = new StringBuilder();

                text.Append("User-agent: *\n");
                text.AppendFormat("Sitemap: {0}\n", context.GetAbsoluteUrl("/sitemap.xml"));

                context.Response.ContentType = "text/plain";
                context.Response.Headers.Add("Cache-Control", new StringValues("public, max-age=86400"));

                await context.Response.WriteAsync(text.ToString());

                return;
            }

            await _next(context);
        }
    }
}