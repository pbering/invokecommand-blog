using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Blog.Data.Exceptions;
using Blog.Data.IO;

namespace Blog.Data
{
    public class Post
    {
        private readonly MarkdownFile _file;

        public Post(MarkdownFile file)
        {
            _file = file;

            Name = _file.Name;
            Url = new Uri("/posts/" + _file.Name.ToLowerInvariant(), UriKind.Relative);
            Tags = new ReadOnlyCollection<Tag>(Enumerable.Empty<Tag>().ToList());
        }

        public string Title { get; private set; }
        public DateTime Published { get; private set; }
        public string Content { get; private set; }
        public Uri Url { get; private set; }
        public ReadOnlyCollection<Tag> Tags { get; private set; }
        public string Name { get; private set; }
        public string Summary { get; private set; }

        public void Parse()
        {
            Title = _file.GetHeaderValue("title");
            Summary = _file.GetHeaderValue("summary");

            var dateString = _file.GetHeaderValue("date");

            DateTime date;

            if (DateTime.TryParse(dateString, out date))
            {
                Published = date;
            }
            else
            {
                throw new InvalidHeaderValueException($"The header 'date' with value '{dateString}' could not be parsed as DateTime");
            }

            var tagsString = _file.GetHeaderValue("tags", true);

            if (!string.IsNullOrWhiteSpace(tagsString))
            {
                var tags = new List<Tag>();

                if (tagsString.IndexOf(",", StringComparison.Ordinal) > -1)
                {
                    tags = tagsString.Split(',')
                        .Where(tagName => !string.IsNullOrEmpty(tagName.Trim()))
                        .Select(tagName => new Tag(tagName.Trim())).ToList();
                }
                else
                {
                    tags.Add(new Tag(tagsString.Trim()));
                }

                Tags = new ReadOnlyCollection<Tag>(tags);
            }

            Content = _file.Body;
        }
    }
}