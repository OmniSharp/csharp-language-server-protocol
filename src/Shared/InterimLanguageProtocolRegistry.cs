using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;

namespace OmniSharp.Extensions.LanguageServer.Shared
{
    internal class InterimLanguageProtocolRegistry<T> : InterimJsonRpcServerRegistry<T> where T : IJsonRpcHandlerRegistry<T>
    {
        private readonly IResolverContext _resolverContext;
        private readonly TextDocumentIdentifiers _textDocumentIdentifiers;

        public InterimLanguageProtocolRegistry(IResolverContext resolverContext, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            handlersManager
        )
        {
            _resolverContext = resolverContext;
            _textDocumentIdentifiers = textDocumentIdentifiers;
        }

        public IDisposable AddTextDocumentIdentifier(params ITextDocumentIdentifier[] identifiers) => _textDocumentIdentifiers.Add(identifiers);

        public IDisposable AddTextDocumentIdentifier<TTextDocumentIdentifier>() where TTextDocumentIdentifier : ITextDocumentIdentifier =>
            _textDocumentIdentifiers.Add(_resolverContext.Resolve<TTextDocumentIdentifier>());
    }
}
