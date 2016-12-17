using System;

namespace Blog.Data.Exceptions
{
    public class InvalidHeaderException : Exception
    {
        public InvalidHeaderException()
        {
        }

        public InvalidHeaderException(string message) : base(message)
        {
        }

        public InvalidHeaderException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}