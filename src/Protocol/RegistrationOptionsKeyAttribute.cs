using System;
using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RegistrationOptionsKeyAttribute : Attribute
    {
        public RegistrationOptionsKeyAttribute(string key, params string[] keys)
        {
            Key = new[] { key }.Concat(keys).ToArray();
        }

        public string[] Key { get; }
    }
}
