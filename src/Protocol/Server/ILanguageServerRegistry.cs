using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerRegistry : IJsonRpcHandlerRegistry<ILanguageServerRegistry>
    {
    }
}
