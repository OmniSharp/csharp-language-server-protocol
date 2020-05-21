using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class ClientLanguageServer : ServerProxyBase, IClientLanguageServer
    {
        public ClientLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
