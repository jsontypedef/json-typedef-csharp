using System;
using System.Linq;
using System.Collections.Generic;

namespace Jtd.Jtd
{
    /// <summary>
    /// Class <c>ValidationError</c> represents a JSON Type Definition
    /// validation error indicator. It is not a CSharp exception.
    /// </summary>
    ///
    /// <seealso cref="Validator.Validate(Schema, IJson)" />
    public class ValidationError
    {
        /// <value>
        /// The path to the part of the instance ("input") that was rejected.
        /// </value>
        public IList<string> InstancePath { get; set; }

        /// <value>
        /// The path to the part of the schema that rejected the input.
        /// </value>
        public IList<string> SchemaPath { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ValidationError error &&
                InstancePath.SequenceEqual(error.InstancePath) &&
                SchemaPath.SequenceEqual(error.SchemaPath);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InstancePath, SchemaPath);
        }
    }
}
