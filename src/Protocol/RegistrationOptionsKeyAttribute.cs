using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    /// <summary>
    /// Defines a converter that is used for converting from dynamic to static
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
//    [Conditional("CodeGeneration")]
    public class RegistrationOptionsKeyAttribute : Attribute
    {
        public RegistrationOptionsKeyAttribute(string serverCapabilitiesKey)
        {
            ServerCapabilitiesKey = serverCapabilitiesKey;
        }

        public string ServerCapabilitiesKey { get; }
    }
}