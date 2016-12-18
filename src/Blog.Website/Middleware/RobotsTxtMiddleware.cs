using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
                text.AppendFormat("Sitemap: {0}://{1}/sitemap.xml\n", context.Request.Scheme, context.Request.Host.Value);

                await context.Response.WriteAsync(text.ToString());

                return;
            }

            await _next(context);
        }
    }
}