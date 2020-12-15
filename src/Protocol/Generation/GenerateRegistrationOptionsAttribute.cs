using System;
using System.Diagnostics;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateRegistrationOptionsAttribute : Attribute
    {
        public bool SupportsWorkDoneProgress { get; init; }
        public bool SupportsStaticRegistrationOptions { get; init; }
        public bool SupportsDocumentSelector { get; init; }
        public Type? Converter { get; init; }

        public bool SupportsTextDocument
        {
            get => SupportsDocumentSelector;
            init => SupportsDocumentSelector = value;
        }

        public GenerateRegistrationOptionsAttribute(string? key = null, params string?[] keys)
        {
            Keys = new [] { key} .Concat(keys).ToArray();
        }

        public string?[] Keys { get; set; }
    }
}
