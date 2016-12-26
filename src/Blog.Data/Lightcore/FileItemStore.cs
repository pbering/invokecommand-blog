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
        private readonly List<Item> _items;

        public FileItemStore(IHostingEnvironment environment)
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
                _items.Add(NewItem(tag, "/home/tags/" + tag, "Tag", new[] {new Field("title", Guid.NewGuid(), tag)}));
            }

            _items.Add(NewItem("Home", "/home", "Posts", new[] {new Field("title", Guid.NewGuid(), "Home")}));
            _items.Add(NewItem("Posts", "/home/posts", children: posts.OrderByDescending(item => item["date"])));
        }

        public Task<Item> GetVersionAsync(GetVersionQuery query)
        {
            Item found;

            Guid id;

            if (Guid.TryParse(query.PathOrId, out id))
            {
                found = _items.FirstOrDefault(item => item.Id == id);
            }
            else
            {
                found = _items.FirstOrDefault(item => item.Path == "/" + query.PathOrId);
            }

            return Task.FromResult(found);
        }

        public Task<IEnumerable<Item>> GetVersionsAsync(GetVersionsQuery query)
        {
            throw new NotSupportedException();
        }

        private Item NewItem(string name, string path, string component = null,
                             IEnumerable<Field> fields = null,
                             IEnumerable<IItemDefinition> children = null)
        {
            if (fields == null)
            {
                fields = new List<Field>(0);
            }

            if (children == null)
            {
                children = new List<IItemDefinition>(0);
            }

            return new Item(new MutableItemDef(name, path, fields), children, NewPresentationDetails(component));
        }

        private PresentationDetails NewPresentationDetails(string component)
        {
            if (component == null)
            {
                return null;
            }

            return new PresentationDetails(new Layout("/Views/Layout.cshtml"), new List<Rendering>(new[]
            {
                new Rendering("main", string.Empty, component, new Dictionary<string, string>(), new Caching(true, true, false, false))
            }));
        }

        private class MutableItemDef : IItemDefinition
        {
            public MutableItemDef(string name, string path, IEnumerable<Field> fields) : this(name, path)
            {
                Fields = new FieldCollection(fields);
            }

            private MutableItemDef(string name, string path)
            {
                Fields = new FieldCollection(Enumerable.Empty<Field>());
                Language = Language.EnglishNeutral;
                Id = Guid.NewGuid();
                Name = name;
                Key = name.ToLowerInvariant();
                HasVersion = true;
                Path = path.ToLowerInvariant();
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