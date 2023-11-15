using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Blog.Website.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Blog.Website.Middleware
{
    public class SitemapMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPostRepository _postRepository;

        public SitemapMiddleware(RequestDelegate next, IPostRepository postRepository)
        {
            _next = next;
            _postRepository = postRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(new PathString("/sitemap.xml"), StringComparison.OrdinalIgnoreCase))
            {
                var posts = _postRepository.Get();

                XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

                var xml = new XDocument(
                                        new XDeclaration("1.0", "utf-8", null),
                                        new XElement(ns + "urlset",
                                                     new XElement(ns + "url",
                                                                  new XElement(ns + "loc", context.GetAbsoluteUrl("/")),
                                                                  new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                                                                  new XElement(ns + "changefreq", "daily")),
                                                     from post in posts
                                                     select
                                                         new XElement(ns + "url",
                                                                      new XElement(ns + "loc", context.GetAbsoluteUrl(post.Url)),
                                                                      new XElement(ns + "lastmod", post.Published.ToString("yyyy-MM-dd")),
                                                                      new XElement(ns + "changefreq", "daily"))
                                                    )
                                       );

                context.Response.ContentType = "text/xml";
                context.Response.Headers.Append("Cache-Control", new StringValues("public, max-age=86400"));

                await context.Response.WriteAsync(xml.ToString());

                return;
            }

            await _next(context);
        }
    }
}