using System;
using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Used by source generation to identify the resolver of a given type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class ResolverAttribute : Attribute
    {
        public Type ResolverType { get; }

        public ResolverAttribute(Type resolverType)
        {
            ResolverType = resolverType;
        }
    }
}