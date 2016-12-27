using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blog.Website.Data.Markdown;
using Blog.Website.Models;
using Microsoft.AspNetCore.Hosting;

namespace Blog.Website.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly List<PostModel> _posts;

        public PostRepository(IHostingEnvironment environment)
        {
            _posts = new List<PostModel>();

            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");

            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                file.Parse();

                var post = new PostModel
                {
                    Name = file.Name,
                    Published = DateTime.Parse(file.Fields["date"]),
                    Title = file.Fields["title"],
                    Text = file.Body,
                    Tags = file.Fields["tags"].Split(',').Select(t => t.Trim()).ToArray(),
                    Summary = file.Fields["summary"],
                    Url = "/posts/" + file.Name
                };

                _posts.Add(post);
            }
        }

        public IReadOnlyCollection<PostModel> Get()
        {
            return _posts;
        }
    }
}