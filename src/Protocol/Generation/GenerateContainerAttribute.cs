using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Allows generating a typed container counterpart to any model that implements <see cref="OmniSharp.Extensions.LanguageServer.Protocol.Models.ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    [Conditional("CodeGeneration")]
    public class GenerateContainerAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local
        public GenerateContainerAttribute(string? className = null)
        {

        }
        
        public bool GenerateImplicitConversion { get; set; }
    }
}
