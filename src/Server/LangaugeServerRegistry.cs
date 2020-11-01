using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LangaugeServerRegistry : InterimLanguageProtocolRegistry<ILanguageServerRegistry>, ILanguageServerRegistry
    {
        public LangaugeServerRegistry(IResolverContext resolverContext, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            resolverContext, handlersManager, textDocumentIdentifiers
        )
        {
        }
    }
}
