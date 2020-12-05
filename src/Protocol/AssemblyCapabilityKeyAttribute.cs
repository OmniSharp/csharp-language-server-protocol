using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class AssemblyCapabilityKeyAttribute : Attribute
    {
        public Type CapabilityType { get; }
        public string CapabilityKey { get; }

        public AssemblyCapabilityKeyAttribute(Type capabilityType, params string[] keys)
        {
            CapabilityType = capabilityType;
            CapabilityKey = string.Join(".", keys);
        }
    }
}
