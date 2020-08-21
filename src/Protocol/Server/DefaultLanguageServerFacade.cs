using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    internal class DefaultLanguageServerFacade : LanguageProtocolProxy, ILanguageServerFacade
    {
        public DefaultLanguageServerFacade(
            IResponseRouter requestRouter, IServiceProvider serviceProvider, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings, ITextDocumentLanguageServer textDocument, IClientLanguageServer client, IGeneralLanguageServer general, IWindowLanguageServer window, IWorkspaceLanguageServer workspace
        ) : base(requestRouter, serviceProvider, progressManager, languageProtocolSettings)
        {
            TextDocument = textDocument;
            Client = client;
            General = general;
            Window = window;
            Workspace = workspace;
        }

        public ITextDocumentLanguageServer TextDocument { get; }
        public IClientLanguageServer Client { get; }
        public IGeneralLanguageServer General { get; }
        public IWindowLanguageServer Window { get; }
        public IWorkspaceLanguageServer Workspace { get; }
    }
}
