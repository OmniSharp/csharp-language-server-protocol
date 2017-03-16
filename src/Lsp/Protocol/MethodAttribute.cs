using System;

namespace Lsp.Protocol
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class MethodAttribute : Attribute
    {
        public string Method { get; }

        public MethodAttribute(string method)
        {
            Method = method;
        }
    }
}