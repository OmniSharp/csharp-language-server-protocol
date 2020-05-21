using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class GeneralLanguageClient : ClientProxyBase, IGeneralLanguageClient
    {
        public GeneralLanguageClient(IClientProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
