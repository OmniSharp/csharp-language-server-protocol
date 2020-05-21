using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class TextDocumentLanguageServer : ServerProxyBase, ITextDocumentLanguageServer
    {
        public TextDocumentLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
