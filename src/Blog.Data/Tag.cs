using System;
using System.Linq;

namespace Blog.Data
{
    public class Tag
    {
        public Tag(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Can not be null or empty", nameof(name));
            }

            Url = new Uri($"/tags/{name.ToLowerInvariant()}", UriKind.Relative);
            Name = name.First().ToString().ToUpper() + string.Join("", name.Skip(1));
        }

        public Uri Url { get; private set; }
        public string Name { get; }

        protected bool Equals(Tag other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Tag)obj);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}