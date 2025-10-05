namespace Blog.Website.Models;

public class HomeModel
{
    public HomeModel(IEnumerable<PostModel> posts)
    {
        Posts = posts;
        Description = "Blog of Per Bering, adventures in code, Sitecore, DevOps and technology.";
    }

    public IEnumerable<PostModel> Posts { get; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Description { get; set; }
}