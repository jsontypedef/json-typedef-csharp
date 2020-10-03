using System;

namespace Jtd.Jtd
{
    /// <summary>
    /// Class <c>InvalidSchemaException</c> is an exception for when a schema is
    /// not correct.
    /// </summary>
    ///
    /// <seealso cref="Schema.Verify" />
    public class InvalidSchemaException : Exception
    {
        public InvalidSchemaException(string message) : base(message)
        {
        }
    }
}
