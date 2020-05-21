using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class GeneralLanguageServer : ServerProxyBase, IGeneralLanguageServer
    {
        public GeneralLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
