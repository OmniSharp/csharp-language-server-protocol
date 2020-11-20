using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.JsonRpc.Generation
{
    /// <summary>
    /// Generates IXyzHandler and XyzHandlerBase classes for a given request object
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateHandlerAttribute : Attribute
    {
        public GenerateHandlerAttribute(string? @namespace = null) { }
    }
}
