using System;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerRegistry : IJsonRpcHandlerRegistry
    {
        IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers);
        IDisposable AddTextDocumentIdentifier<T>() where T : ITextDocumentIdentifier;
    }
}
