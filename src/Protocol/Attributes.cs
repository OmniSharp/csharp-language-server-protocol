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
    public class GenerateTypedDataAttribute : Attribute
    {
    }

    /// <summary>
    /// Allows generating a typed container counterpart to any model that implements <see cref="ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateContainerAttribute : Attribute
    {
    }

    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class CapabilityAttribute : Attribute
    {
        public Type CapabilityType { get; }

        public CapabilityAttribute(Type capabilityType)
        {
            CapabilityType = capabilityType;
        }
    }

    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class RegistrationOptionsKeyAttribute : Attribute
    {
        public RegistrationOptionsKeyAttribute(string serverCapabilitiesKey)
        {
            ServerCapabilitiesKey = serverCapabilitiesKey;
        }

        public string ServerCapabilitiesKey { get; }
    }

    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class RegistrationOptionsAttribute : Attribute
    {
        public Type RegistrationOptionsType { get; }
        public bool SupportsWorkDoneProgress { get; init; }
        public bool SupportsDocumentSelector { get; init; }
        public Type? Converter { get; init; }

        public bool SupportsTextDocument
        {
            get => SupportsDocumentSelector;
            init => SupportsDocumentSelector = value;
        }

        public RegistrationOptionsAttribute(Type registrationOptionsType)
        {
            RegistrationOptionsType = registrationOptionsType;
        }
    }

    /// <summary>
    /// Generates work done on a registration options object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class WorkDoneProgressAttribute : Attribute
    {
    }

    /// <summary>
    /// Generates text document on a registration options object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class TextDocumentAttribute : Attribute
    {
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
