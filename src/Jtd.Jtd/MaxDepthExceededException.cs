using System;

namespace Jtd.Jtd
{
    public class MaxDepthExceededException : Exception
    {
        public MaxDepthExceededException() : base("max depth exceeded during Validator.Validate")
        {
        }
    }
}
