using System;
using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Allows generating a typed container counterpart to any model that implements <see cref="ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [Conditional("CodeGeneration")]
    public class GenerateContainerAttribute : Attribute
    {
        public GenerateContainerAttribute(string? className = null)
        {

        }
    }
}
