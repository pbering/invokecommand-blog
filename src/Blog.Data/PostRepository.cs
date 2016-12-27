using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Markdown;

namespace Blog.Data
{
    public class PostRepository
    {
        private readonly List<Item> _items;

        public PostRepository(IHostingEnvironment environment)
        {
            _items = new List<Item>();

            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");
            var posts = new List<Item>();

            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                file.Parse();

                var fields = file.Headers.Select(header => new Field(header.Key, Guid.NewGuid(), header.Value)).ToList();

                fields.Add(new Field("text", Guid.Empty, file.Body));

                var item = NewItem(file.Name, "/home/posts/" + file.Name, "Post", fields);

                posts.Add(item);

                _items.Add(item);
            }

            var tagNames = new List<string>();

            foreach (var post in posts.Where(p => p.Fields["tags"] != null))
            {
                tagNames.AddRange(post["tags"].Split(',').Select(t => t.Trim()));
            }

            foreach (var tag in tagNames.Distinct())
            {
                _items.Add(NewItem(tag, "/home/tags/" + tag, "Tag", new[] { new Field("title", Guid.NewGuid(), tag) }));
            }

            _items.Add(NewItem("Home", "/home", "Posts", new[] { new Field("title", Guid.NewGuid(), "Home") }));
            _items.Add(NewItem("Posts", "/home/posts", children: posts.OrderByDescending(item => item["date"])));
        }
    }
}
