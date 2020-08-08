using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Shared;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    class LangaugeServerRegistry : InterimLanguageProtocolRegistry<ILanguageServerRegistry>, ILanguageServerRegistry
    {
        public LangaugeServerRegistry(IServiceProvider serviceProvider, CompositeHandlersManager handlersManager, TextDocumentIdentifiers textDocumentIdentifiers) : base(
            serviceProvider, handlersManager, textDocumentIdentifiers)
        {
        }
    }
}
