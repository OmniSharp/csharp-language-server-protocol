using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class WindowLanguageClient : ClientProxyBase, IWindowLanguageClient
    {
        public WindowLanguageClient(IClientProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider)
        {
        }
    }
}
