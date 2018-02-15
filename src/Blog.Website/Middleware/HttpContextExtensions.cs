using System;
using Microsoft.AspNetCore.Http;

namespace Blog.Website.Middleware
{
    public static class HttpContextExtensions
    {
        public static bool AlwaysUseHttp { get; set; }

        public static string GetAbsoluteUrl(this HttpContext context, string path)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.TrimStart('/');

            var hostname = context.Request.Host.Value;
            var scheme = "https";

            if (AlwaysUseHttp)
            {
                scheme = "http";
            }

            return $"{scheme}://{hostname}/{path}";
        }
    }
}