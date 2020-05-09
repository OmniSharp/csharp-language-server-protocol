using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class DynamicCapability
    {
        /// <summary>
        /// Whether completion supports dynamic registration.
        /// </summary>
        [Optional]
        public bool DynamicRegistration { get; set; }
    }
}
