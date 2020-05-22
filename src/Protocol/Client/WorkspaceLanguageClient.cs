using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class WorkspaceLanguageClient : ClientProxyBase, IWorkspaceLanguageClient
    {
        public WorkspaceLanguageClient(IClientProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
