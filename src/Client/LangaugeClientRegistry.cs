using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    internal class LangaugeClientRegistry : InterimLanguageProtocolRegistry<ILanguageClientRegistry>, ILanguageClientRegistry
    {
        public LangaugeClientRegistry(IResolverContext resolverContext, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            resolverContext, handlersManager, textDocumentIdentifiers
        )
        {
        }
    }
}
