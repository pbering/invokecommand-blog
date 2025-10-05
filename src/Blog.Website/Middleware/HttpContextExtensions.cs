namespace Blog.Website.Middleware;

public static class HttpContextExtensions
{
    public static bool AlwaysUseHttp { get; set; }

    public static string GetAbsoluteUrl(this HttpContext context, string path)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(path);

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