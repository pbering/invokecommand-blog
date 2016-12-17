using System;

namespace Blog.Data.Exceptions
{
    public class InvalidHeaderValueException : Exception
    {
        public InvalidHeaderValueException()
        {
        }

        public InvalidHeaderValueException(string message) : base(message)
        {
        }

        public InvalidHeaderValueException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}