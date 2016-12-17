using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blog.Data.IO;

namespace Blog.Data
{
    public class PostRepository : IEnumerable<Post>
    {
        private readonly FileSystem _filesystem;

        public PostRepository(FileSystem filesystem)
        {
            _filesystem = filesystem;
        }

        public IEnumerator<Post> GetEnumerator()
        {
            var data = new List<Post>();

            foreach (var file in _filesystem.GetFiles("*.md"))
            {
                var post = new Post(new MarkdownFile(file));

                post.Parse();

                data.Add(post);
            }

            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Post FindByName(string name)
        {
            return this.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Post> FindByTag(string tagName)
        {
            return this.Where(post => post.Tags.Contains(new Tag(tagName)));
        }
    }
}