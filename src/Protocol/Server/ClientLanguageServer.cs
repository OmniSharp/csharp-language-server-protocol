using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class ClientLanguageServer : ServerProxyBase, IClientLanguageServer
    {
        public ClientLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
