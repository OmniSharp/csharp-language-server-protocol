using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Generation
{
    /// <summary>
    /// Used by source generation to identify the registration options type to use
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [Conditional("CodeGeneration")]
    public class RegistrationOptionsAttribute : Attribute
    {
        public Type RegistrationOptionsType { get; }

        public RegistrationOptionsAttribute(Type registrationOptionsType)
        {
            RegistrationOptionsType = registrationOptionsType;
        }
    }
}
