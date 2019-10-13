using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public abstract class LinkSupportClientCapabilities : DynamicCapability
    {
        [Optional]
        public bool LinkSupport { get; set; }
    }
}
