using System;
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Allows generating a typed counterpart to any model that implements <see cref="ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateTypedDataAttribute : Attribute { }

    /// <summary>
    /// Allows generating a typed container counterpart to any model that implements <see cref="ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateContainerAttribute : Attribute { }

    /// <summary>
    /// Generates work done on a registration options object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class WorkDoneProgressAttribute : Attribute { }

    /// <summary>
    /// Generates text document on a registration options object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class TextDocumentAttribute : Attribute { }

    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class RegistrationOptionsAttribute : Attribute
    {
        public string ServerCapabilitiesKey { get; }

        public RegistrationOptionsAttribute(string serverCapabilitiesKey)
        {
            ServerCapabilitiesKey = serverCapabilitiesKey;
        }
    }

    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class RegistrationOptionsConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public RegistrationOptionsConverterAttribute(Type converterType)
        {
            ConverterType = converterType;
        }
    }
}
