using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class GenerateRegistrationOptionsAttribute : Attribute
    {
        public string? ServerCapabilitiesKey { get; }
        public bool SupportsWorkDoneProgress { get; init; }
        public bool SupportsStaticRegistrationOptions { get; init; }
        public bool SupportsDocumentSelector { get; init; }
        public Type? Converter { get; init; }

        public bool SupportsTextDocument
        {
            get => SupportsDocumentSelector;
            init => SupportsDocumentSelector = value;
        }

        public GenerateRegistrationOptionsAttribute(string? serverCapabilitiesKey = null)
        {
            ServerCapabilitiesKey = serverCapabilitiesKey;
        }
    }
}
