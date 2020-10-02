using System;

namespace Jtd.Jtd
{
    public class InvalidSchemaException : Exception
    {
        public InvalidSchemaException(string message) : base(message)
        {
        }
    }
}
