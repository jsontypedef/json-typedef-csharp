using System;
using System.Linq;
using System.Collections.Generic;

namespace Jtd.Jtd
{
    public class ValidationError
    {
        public IList<string> InstancePath { get; set; }
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
