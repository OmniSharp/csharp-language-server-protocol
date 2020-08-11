using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public interface ISerializer : JsonRpc.ISerializer
    {
        void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities);
    }
}
