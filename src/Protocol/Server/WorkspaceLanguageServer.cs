using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class WorkspaceLanguageServer : ServerProxyBase, IWorkspaceLanguageServer
    {
        public WorkspaceLanguageServer(IServerProxy proxy, IServiceProvider serviceProvider) : base(proxy, serviceProvider) { }
    }
}
