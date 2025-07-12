using System.Diagnostics;

namespace OmniSharp.Extensions.JsonRpc.Generation
{
    /// <summary>
    /// Generates a string based enum for the given readonly struct
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct)]
    [Conditional("CodeGeneration")]
    public class StringEnumAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local
        public StringEnumAttribute(string? @namespace = null) { }
    }
}
