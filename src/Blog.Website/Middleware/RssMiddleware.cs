using Blog.Website.Data;
using Microsoft.Extensions.Primitives;
using System.Xml.Linq;

namespace Blog.Website.Middleware;

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
            var xml = new XDocument(
                                    new XDeclaration("1.0", "utf-8", null),
                                    new XElement("rss",
                                                 new XAttribute("version", "2.0"),
                                                 new XElement("channel",
                                                              new XElement("link", context.GetAbsoluteUrl("/")),
                                                              new XElement("lastBuildDate", DateTime.Now.ToString("R")),
                                                              new XElement("title", "invokecommand.net"),
                                                              new XElement("description", "All blog posts"),
                                                              new XElement("language", "en-us"),
                                                              from post in posts
                                                              select
                                                                  new XElement("item",
                                                                               new XElement("link", context.GetAbsoluteUrl(post.Url)),
                                                                               new XElement("description", post.Summary),
                                                                               new XElement("title", post.Title),
                                                                               new XElement("pubDate", post.Published.ToString("R")),
                                                                               new XElement("guid", post.Name,
                                                                                            new XAttribute("isPermaLink", "false"))
                                                             ))
                                   ));

            context.Response.ContentType = "text/xml";
            context.Response.Headers.Append("Cache-Control", new StringValues("public, max-age=86400"));

            await context.Response.WriteAsync(xml.ToString(SaveOptions.DisableFormatting));

            return;
        }

        await _next(context);
    }
}