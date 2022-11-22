using System.Collections.Generic;

namespace Blog.Website.Models
{
    public class HomeModel
    {
        public HomeModel(IEnumerable<PostModel> posts)
        {
            Posts = posts;
            Description = "Blog of Per Bering, adventures in code, Sitecore, DevOps and technology.";
        }

        public IEnumerable<PostModel> Posts { get; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
    }
}