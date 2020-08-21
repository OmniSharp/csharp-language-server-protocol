using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    internal class DefaultLanguageClientFacade : LanguageProtocolProxy, ILanguageClientFacade
    {
        public DefaultLanguageClientFacade(
            IResponseRouter requestRouter, IServiceProvider serviceProvider, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings, ITextDocumentLanguageClient textDocument, IClientLanguageClient client, IGeneralLanguageClient general, IWindowLanguageClient window, IWorkspaceLanguageClient workspace
        ) : base(requestRouter, serviceProvider, progressManager, languageProtocolSettings)
        {
            TextDocument = textDocument;
            Client = client;
            General = general;
            Window = window;
            Workspace = workspace;
        }

        public ITextDocumentLanguageClient TextDocument { get; }
        public IClientLanguageClient Client { get; }
        public IGeneralLanguageClient General { get; }
        public IWindowLanguageClient Window { get; }
        public IWorkspaceLanguageClient Workspace { get; }
    }
}