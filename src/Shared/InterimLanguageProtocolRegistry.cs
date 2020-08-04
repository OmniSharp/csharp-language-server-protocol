using System;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    class InterimLanguageProtocolRegistry<T> : InterimJsonRpcServerRegistry<T>  where T : IJsonRpcHandlerRegistry<T>
    {
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        public InterimLanguageProtocolRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(serviceProvider, handlersManager)
        {
            _textDocumentIdentifiers = textDocumentIdentifiers;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] handlers)
        {
            return _textDocumentIdentifiers.Add(handlers);
        }

        public IDisposable AddTextDocumentIdentifier<TTextDocumentIdentifier>() where TTextDocumentIdentifier : ITextDocumentIdentifier
        {
            return _textDocumentIdentifiers.Add(ActivatorUtilities.CreateInstance<TTextDocumentIdentifier>(_serviceProvider));
        }
    }
}
