using Blog.Website.Data.Markdown;
using Blog.Website.Models;
using Markdig;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Blog.Website.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly IEnumerable<PostModel> _posts;

        public PostRepository(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline)
        {
            var posts = new List<PostModel>();
            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");

            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                file.Parse(markdownPipeline);

                var post = new PostModel
                {
                    Name = file.Name,
                    Published = DateTime.Parse(file.Fields["date"]),
                    Title = file.Fields["title"],
                    Text = file.Body,
                    Tags = file.Fields["tags"].Split(',').Select(t => t.Trim().ToLowerInvariant()).ToArray(),
                    Summary = file.Fields["summary"],
                    Url = "/posts/" + file.Name
                };

                posts.Add(post);
            }

            _posts = posts.OrderByDescending(p => p.Published);
        }

        public IEnumerable<PostModel> Get()
        {
            return _posts;
        }
    }
}