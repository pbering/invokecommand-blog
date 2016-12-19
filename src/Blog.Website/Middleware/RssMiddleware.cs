using System;
using System.Globalization;
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
    public class RssMiddleware
    {
        private readonly IItemStore _itemStore;
        private readonly RequestDelegate _next;
        private readonly IItemUrlService _urlService;

        public RssMiddleware(RequestDelegate next, IItemStore itemStore, IItemUrlService urlService)
        {
            _next = next;
            _itemStore = itemStore;
            _urlService = urlService;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(new PathString("/rss.xml"), StringComparison.OrdinalIgnoreCase))
            {
                var posts = await _itemStore.GetVersionAsync("/home/posts", Language.EnglishNeutral);

                XNamespace ns = "http://www.w3.org/2005/Atom";

                var xml = new XDocument(
                                        new XDeclaration("1.0", "utf-8", null),
                                        new XElement(ns + "rss",
                                                     new XAttribute("version", "2.0"),
                                                     new XElement(ns + "channel",
                                                                  new XElement(ns + "link", context.Request.Scheme + "://" + context.Request.Host.Value),
                                                                  new XElement(ns + "lastBuildDate", DateTime.Now.ToString("R")),
                                                                  new XElement(ns + "title", "invokecommand.net"),
                                                                  new XElement(ns + "description", "All blog posts"),
                                                                  new XElement(ns + "language", "en-us"),
                                                                  from post in posts.Children
                                                                  select
                                                                  new XElement(ns + "item",
                                                                               new XElement(ns + "link",
                                                                                            context.Request.Scheme + "://" +
                                                                                            context.Request.Host.Value +
                                                                                            _urlService.GetUrl(post)),
                                                                               new XElement(ns + "description", post["summary"]),
                                                                               new XElement(ns + "title", post["title"]),
                                                                               new XElement(ns + "updated", DateTime.Parse(post["date"]).ToString("R")),
                                                                               new XElement(ns + "guid", post.Key, new XAttribute("isPermaLink", "false")))
                                                                 ))
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