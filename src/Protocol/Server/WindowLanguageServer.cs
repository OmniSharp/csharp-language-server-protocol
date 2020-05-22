using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WindowLanguageServer : ServerProxyBase, IWindowLanguageServer
    {
        public WindowLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider)
        {
        }
    }
}
