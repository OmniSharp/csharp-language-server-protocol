using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Identifies this handler as a "builtin" handler that that will be dropped if a non-built in one is given
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class BuiltInAttribute : Attribute
    {
    }
}
