using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightcore.Kernel.Data;
using Lightcore.Kernel.Data.Fields;
using Lightcore.Kernel.Data.Globalization;
using Lightcore.Kernel.Data.Presentation;
using Lightcore.Kernel.Data.Storage;

namespace Blog.Data.Lightcore
{
    public class FileItemStore : IItemStore
    {
        private readonly Dictionary<string, Item> _items;

        public FileItemStore()
        {
            var layout = new Layout("/Views/Layout.cshtml");

            _items = new Dictionary<string, Item>
            {
                {
                    "/home",
                    new Item(new BlogData("Home", "/home"),
                             details:
                             new PresentationDetails(layout,
                                                     new List<Rendering>(new[]
                                                     {
                                                         new Rendering("main", string.Empty, "Posts", new Dictionary<string, string>(),
                                                                       new Caching(false, false, false, false))
                                                     })))
                },
                {
                    "/home/tags/docker",
                    new Item(new BlogData("docker", "/home/tags/docker"),
                             details: new PresentationDetails(layout, new List<Rendering>(new[]
                             {
                                 new Rendering("main", string.Empty, "Tag", new Dictionary<string, string>(),
                                               new Caching(false, false, false, false))
                             })))
                }
                ,
                {
                    "/home/posts/this-is-a-post",
                    new Item(new BlogData("this-is-a-post", "/home/posts/this-is-a-post"),
                             details: new PresentationDetails(layout, new List<Rendering>(new[]
                             {
                                 new Rendering("main", string.Empty, "Post", new Dictionary<string, string>(),
                                               new Caching(false, false, false, false))
                             })))
                }

                // TODO: Generate "tag" items i stedet for at lave wildcard!
                // TODO: Generate "post" items i stedet for at lave wildcard!
            };
        }

        public Task<Item> GetVersionAsync(GetVersionQuery query)
        {
            Item item;

            _items.TryGetValue("/" + query.PathOrId, out item);

            return Task.FromResult(item);
        }

        public Task<IEnumerable<Item>> GetVersionsAsync(GetVersionsQuery query)
        {
            throw new NotSupportedException();
        }

        private class BlogData : IItemDefinition
        {
            public BlogData(string name, string path)
            {
                Fields = new FieldCollection(Enumerable.Empty<Field>());
                Language = Language.EnglishNeutral;
                Id = Guid.NewGuid();
                Name = name;
                Key = name.ToLowerInvariant();
                HasVersion = true;
                Path = path;
                ParentId = Guid.Empty;
                TemplateId = Guid.Empty;
                RevisionTag = "1";
            }

            public Guid TemplateId { get; }
            public Guid Id { get; }
            public string Name { get; }
            public string Key { get; }
            public string Path { get; }
            public string RevisionTag { get; }
            public Language Language { get; }
            public bool HasVersion { get; }
            public FieldCollection Fields { get; }
            public Guid ParentId { get; }
            public string this[string fieldName] => Fields[fieldName]?.Value;
        }
    }
}