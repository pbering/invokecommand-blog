namespace Blog.Website.Models;

public class PostModel
{
    public required string Url { get; set; }
    public required string Title { get; set; }
    public required string Name { get; set; }
    public DateTime Published { get; set; }
    public required string Text { get; set; }
    public required string Summary { get; set; }
    public required string[] Tags { get; set; }
}