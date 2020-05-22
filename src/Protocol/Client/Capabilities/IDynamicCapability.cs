using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public interface IDynamicCapability : ICapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [Optional]
        bool DynamicRegistration { get; set; }
    }
}
