using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WorkspaceLanguageServer : ServerProxyBase, IWorkspaceLanguageServer
    {
        public WorkspaceLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
