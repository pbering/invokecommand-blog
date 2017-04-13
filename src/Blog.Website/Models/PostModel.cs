using System;

namespace Blog.Website.Models
{
    public class PostModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public DateTime Published { get; set; }
        public string Text { get; set; }
        public string Summary { get; set; }
        public string[] Tags { get; set; }
    }
}