using System;

namespace OmniSharp.Extensions.JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Method { get; }
        public Direction Direction { get; }

        public MethodAttribute(string method)
        {
            Method = method;
        }

        /// <summary>
        /// Decorate the method given the given direction
        /// </summary>
        /// <param name="method"></param>
        /// <param name="direction"></param>
        public MethodAttribute(string method, Direction direction)
        {
            Method = method;
            Direction = direction;
        }
    }
}
