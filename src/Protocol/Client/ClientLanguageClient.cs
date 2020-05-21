using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class ClientLanguageClient : ClientProxyBase, IClientLanguageClient
    {
        public ClientLanguageClient(IClientProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
