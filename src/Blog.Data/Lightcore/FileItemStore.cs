using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blog.Data.Markdown;
using Lightcore.Kernel.Data;
using Lightcore.Kernel.Data.Fields;
using Lightcore.Kernel.Data.Globalization;
using Lightcore.Kernel.Data.Presentation;
using Lightcore.Kernel.Data.Storage;
using Microsoft.AspNetCore.Hosting;

namespace Blog.Data.Lightcore
{
    public class FileItemStore : IItemStore
    {
        private readonly Dictionary<string, Item> _items;

        public FileItemStore(IHostingEnvironment environment)
        {
            _items = new Dictionary<string, Item>();

            var layout = new Layout("/Views/Layout.cshtml");
            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");
            var posts = new List<Item>();

            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                file.Parse();

                var fields = file.Headers.Select(header => new Field(header.Key, Guid.Empty, header.Value)).ToList();

                fields.Add(new Field("text", Guid.Empty, file.Body));

                var path = "/home/posts/" + file.Name;
                var item = new Item(new MutableItemDef(file.Name, path, fields),
                                    details: new PresentationDetails(layout, new List<Rendering>(new[]
                                    {
                                        new Rendering("main", string.Empty, "Post", new Dictionary<string, string>(),
                                                      new Caching(false, false, false, false))
                                    })));

                posts.Add(item);

                _items.Add(path, item);
            }

            _items.Add("/home", new Item(new MutableItemDef("Home", "/home", new[] {new Field("title", Guid.Empty, "Home")}),
                                         details:
                                         new PresentationDetails(layout,
                                                                 new List<Rendering>(new[]
                                                                 {
                                                                     new Rendering("main", string.Empty, "Posts", new Dictionary<string, string>(),
                                                                                   new Caching(false, false, false, false))
                                                                 }))));

            _items.Add("/home/posts",
                       new Item(new MutableItemDef("Posts", "/home/posts"), posts.OrderByDescending(item => item["date"]),
                                new PresentationDetails(layout, new List<Rendering>())));
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

        private class MutableItemDef : IItemDefinition
        {
            public MutableItemDef(string name, string path, IEnumerable<Field> fields) : this(name, path)
            {
                Fields = new FieldCollection(fields);
            }

            public MutableItemDef(string name, string path)
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