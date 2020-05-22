using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class TextDocumentLanguageClient : ClientProxyBase, ITextDocumentLanguageClient
    {
        public TextDocumentLanguageClient(IClientProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
