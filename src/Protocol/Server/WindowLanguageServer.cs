using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WindowLanguageServer : ServerProxyBase, IWindowLanguageServer
    {
        public WindowLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider)
        {
        }
    }
}
