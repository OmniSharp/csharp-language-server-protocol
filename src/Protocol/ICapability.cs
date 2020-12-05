using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ICapability<in TCapability>
    {
        void SetCapability(TCapability capability, ClientCapabilities clientCapabilities);
    }
}
