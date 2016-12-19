using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lightcore.Kernel.Data.Globalization;
using Lightcore.Kernel.Data.Storage;
using Lightcore.Kernel.Url;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Blog.Website.Middleware
{
    public class SitemapMiddleware
    {
        private readonly IItemStore _itemStore;
        private readonly RequestDelegate _next;
        private readonly IItemUrlService _urlService;

        public SitemapMiddleware(RequestDelegate next, IItemStore itemStore, IItemUrlService urlService)
        {
            _next = next;
            _itemStore = itemStore;
            _urlService = urlService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(new PathString("/sitemap.xml"), StringComparison.OrdinalIgnoreCase))
            {
                var posts = await _itemStore.GetVersionAsync("/home/posts", Language.EnglishNeutral);

                XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

                var xml = new XDocument(
                                        new XDeclaration("1.0", "utf-8", null),
                                        new XElement(ns + "urlset",
                                                     new XElement(ns + "url",
                                                                  new XElement(ns + "loc", context.Request.Scheme + "://" + context.Request.Host.Value),
                                                                  new XElement(ns + "lastmod", DateTime.Now.ToString("yyyy-MM-dd")),
                                                                  new XElement(ns + "changefreq", "daily")),
                                                     from post in posts.Children
                                                     select
                                                     new XElement(ns + "url",
                                                                  new XElement(ns + "loc",
                                                                               context.Request.Scheme + "://" + context.Request.Host.Value +
                                                                               _urlService.GetUrl(post)),
                                                                  new XElement(ns + "lastmod", post["date"]),
                                                                  new XElement(ns + "changefreq", "daily"))
                                                    )
                                       );

                context.Response.ContentType = "text/xml";
                context.Response.Headers.Add("Cache-Control", new StringValues("public, max-age=86400"));

                await context.Response.WriteAsync(xml.ToString());

                return;
            }

            await _next(context);
        }
    }
}