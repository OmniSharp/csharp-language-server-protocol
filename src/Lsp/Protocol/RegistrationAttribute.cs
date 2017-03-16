using System;
using Lsp.Methods;

namespace Lsp.Protocol
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class RegistrationAttribute : Attribute
    {
        public Type Type { get; }
        public RegistrationAttribute(Type type)
        {
            Type = type;
        }
    }
}