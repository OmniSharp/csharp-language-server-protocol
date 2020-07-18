using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientRegistry : IJsonRpcHandlerRegistry<ILanguageClientRegistry>
    {
    }
}
