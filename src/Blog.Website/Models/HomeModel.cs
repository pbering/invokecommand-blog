using System.Collections.Generic;

namespace Blog.Website.Models
{
    public class HomeModel
    {
        public HomeModel(IEnumerable<PostModel> posts)
        {
            Posts = posts;
        }

        public IEnumerable<PostModel> Posts { get; }
    }
}