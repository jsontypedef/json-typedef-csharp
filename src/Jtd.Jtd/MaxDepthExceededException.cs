using System;

namespace Jtd.Jtd
{
    /// <summary>
    /// Class <c>MaxDepthExceededException</c> is an exception for when
    /// validation follows too many <c>ref</c>s.
    /// </summary>
    ///
    /// <seealso cref="Validator.Validate" />
    public class MaxDepthExceededException : Exception
    {
        public MaxDepthExceededException() : base("max depth exceeded during Validator.Validate")
        {
        }
    }
}
