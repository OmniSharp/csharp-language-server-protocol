using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClientRegistry : IJsonRpcHandlerRegistry
    {
        IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers);
        IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier;

        IDisposable AddHandler<T>(Func<IServiceProvider, T> handlerFunc) where T : IJsonRpcHandler;
    }
}
