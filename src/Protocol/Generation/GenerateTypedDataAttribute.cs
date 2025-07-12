using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Allows generating a typed counterpart to any model that implements <see cref="OmniSharp.Extensions.LanguageServer.Protocol.Models.ICanBeResolved" />
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateTypedDataAttribute : Attribute
    {
    }
}
