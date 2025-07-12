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
        // ReSharper disable once UnusedParameter.Local
        public GenerateHandlerAttribute(string? @namespace = null) { }

        public string? Name { get; init; }

        /// <summary>
        /// Allow the request to be derived and create methods that take a request type argument.
        /// </summary>
        public bool AllowDerivedRequests { get; set; }
    }
}
