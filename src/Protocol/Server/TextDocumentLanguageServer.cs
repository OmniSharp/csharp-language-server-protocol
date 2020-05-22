using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class TextDocumentLanguageServer : ServerProxyBase, ITextDocumentLanguageServer
    {
        public TextDocumentLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
