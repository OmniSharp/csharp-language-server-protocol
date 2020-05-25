using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ILanguageProtocolRpcOptions<T> : IJsonRpcHandlerRegistry<T>, IJsonRpcServerOptions
        where T : ILanguageProtocolRpcOptions<T>
    {
        T AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers);
        T AddTextDocumentIdentifier<TTextDocumentIdentifier>() where TTextDocumentIdentifier : ITextDocumentIdentifier;
    }
}
