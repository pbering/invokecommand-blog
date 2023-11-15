using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Blog.Website.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Blog.Website.Middleware
{
    public class RssMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPostRepository _postRepository;

        public RssMiddleware(RequestDelegate next, IPostRepository postRepository)
        {
            _next = next;
            _postRepository = postRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals(new PathString("/rss.xml"), StringComparison.OrdinalIgnoreCase))
            {
                var posts = _postRepository.Get();

                XNamespace ns = "http://www.w3.org/2005/Atom";

                var xml = new XDocument(
                                        new XDeclaration("1.0", "utf-8", null),
                                        new XElement(ns + "rss",
                                                     new XAttribute("version", "2.0"),
                                                     new XElement(ns + "channel",
                                                                  new XElement(ns + "link", context.GetAbsoluteUrl("/")),
                                                                  new XElement(ns + "lastBuildDate", DateTime.Now.ToString("R")),
                                                                  new XElement(ns + "title", "invokecommand.net"),
                                                                  new XElement(ns + "description", "All blog posts"),
                                                                  new XElement(ns + "language", "en-us"),
                                                                  from post in posts
                                                                  select
                                                                      new XElement(ns + "item",
                                                                                   new XElement(ns + "link", context.GetAbsoluteUrl(post.Url)),
                                                                                   new XElement(ns + "description", post.Summary),
                                                                                   new XElement(ns + "title", post.Title),
                                                                                   new XElement(ns + "updated", post.Published.ToString("R")),
                                                                                   new XElement(ns + "guid", post.Name,
                                                                                                new XAttribute("isPermaLink", "false")))
                                                                 ))
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