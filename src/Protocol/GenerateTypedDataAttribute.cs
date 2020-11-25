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
    /// Identifies this handler as a "builtin" handler that that will be dropped if a non-built in one is given
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BuiltInAttribute : Attribute
    {
    }
}
