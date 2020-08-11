using System;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class InterimLanguageProtocolRegistry<T> : InterimJsonRpcServerRegistry<T> where T : IJsonRpcHandlerRegistry<T>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        public InterimLanguageProtocolRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            handlersManager
        )
        {
            _serviceProvider = serviceProvider;
            _textDocumentIdentifiers = textDocumentIdentifiers;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] identifiers) => _textDocumentIdentifiers.Add(identifiers);

        public IDisposable AddTextDocumentIdentifier<TTextDocumentIdentifier>() where TTextDocumentIdentifier : ITextDocumentIdentifier =>
            _textDocumentIdentifiers.Add(ActivatorUtilities.CreateInstance<TTextDocumentIdentifier>(_serviceProvider));
    }
}
