using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class GeneralLanguageServer : ServerProxyBase, IGeneralLanguageServer
    {
        public GeneralLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
