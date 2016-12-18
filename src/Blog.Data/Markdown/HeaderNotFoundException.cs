using System;

namespace Blog.Data.Markdown
{
    public class HeaderNotFoundException : Exception
    {
        public HeaderNotFoundException()
        {
        }

        public HeaderNotFoundException(string message) : base(message)
        {
        }

        public HeaderNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}