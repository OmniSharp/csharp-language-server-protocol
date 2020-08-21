using System;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public sealed class CapabilityKeyAttribute : Attribute
    {
        public IEnumerable<string> Keys { get; }

        public CapabilityKeyAttribute(string key, params string[] additionalKeys)
        {
            Keys = new[] { key }.Concat(additionalKeys).ToArray();
        }
    }
}
