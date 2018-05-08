using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public interface ISerializer : OmniSharp.Extensions.JsonRpc.ISerializer
    {
        void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities);
    }
}
