using System;
using System.Diagnostics;
using CodeGeneration.Roslyn;

namespace OmniSharp.Extensions.JsonRpc.Generation
{
    /// <summary>
    /// Allows generating OnXyz handler methods for a given IJsonRpcHandler
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface)]
    [CodeGenerationAttribute("OmniSharp.Extensions.JsonRpc.Generators.GenerateHandlerMethodsGenerator, OmniSharp.Extensions.JsonRpc.Generators")]
    [Conditional("CodeGeneration")]
    public class GenerateHandlerMethodsAttribute : Attribute
    {
        public GenerateHandlerMethodsAttribute(params Type[] registryTypes)        { }
    }
}
