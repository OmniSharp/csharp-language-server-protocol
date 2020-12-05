using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.JsonRpc.Generation
{
    /// <summary>
    /// Allows generating SendXyz/RequestXyz methods for a given IJsonRpcHandler
    /// </summary>
    /// <remarks>
    /// Efforts will be made to make this available for consumers once source generators land
    /// </remarks>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class GenerateRequestMethodsAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local
        public GenerateRequestMethodsAttribute(params Type[] proxyTypes)
        {
        }

        public string? MethodName { get; set; }
    }
}
