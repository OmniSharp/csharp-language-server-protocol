using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Used by source generation to identify the capability type to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class CapabilityAttribute : Attribute
    {
        public Type CapabilityType { get; }

        public CapabilityAttribute(Type capabilityType)
        {
            CapabilityType = capabilityType;
        }
    }
}
