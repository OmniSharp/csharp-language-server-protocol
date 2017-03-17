using System;

namespace JsonRpc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
        public string Method { get; }

        public MethodAttribute(string method)
        {
            Method = method;
        }
    }
}