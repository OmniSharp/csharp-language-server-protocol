﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    // TODO: maybe?
    // [Conditional("GeneratedCode")]
    public sealed class CapabilityKeyAttribute : Attribute
    {
        public IEnumerable<string> Keys { get; }

        public CapabilityKeyAttribute(string key, params string[] additionalKeys)
        {
            Keys = new[] { key }.Concat(additionalKeys).ToArray();
        }
    }
}
