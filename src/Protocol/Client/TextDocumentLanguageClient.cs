using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    internal class TextDocumentLanguageClient : LanguageProtocolProxy, ITextDocumentLanguageClient
    {
        public TextDocumentLanguageClient(IResponseRouter requestRouter, IServiceProvider serviceProvider, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings) : base(requestRouter, serviceProvider, progressManager, languageProtocolSettings)
        {
        }
    }
}
