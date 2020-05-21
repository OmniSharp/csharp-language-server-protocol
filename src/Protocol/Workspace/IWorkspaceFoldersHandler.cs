using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
    public interface IWorkspaceFoldersHandler : IJsonRpcRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>> { }

    public abstract class WorkspaceFoldersHandler : IWorkspaceFoldersHandler
    {
        public abstract Task<Container<WorkspaceFolder>> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken);
    }

    public static class WorkspaceFoldersExtensions
    {
        public static IDisposable OnWorkspaceFolders(
            this ILanguageClientRegistry registry,
            Func<WorkspaceFolderParams, CancellationToken, Task<Container<WorkspaceFolder>>>
                handler)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceFolders, RequestHandler.For(handler));
        }

        public static IDisposable OnWorkspaceFolders(
            this ILanguageClientRegistry registry,
            Func<WorkspaceFolderParams, Task<Container<WorkspaceFolder>>> handler)
        {
            return registry.AddHandler(WorkspaceNames.WorkspaceFolders, RequestHandler.For(handler));
        }

        public static Task<Container<WorkspaceFolder>> RequestWorkspaceFolders(this IWorkspaceLanguageServer mediator, WorkspaceFolderParams workspaceFolderParams, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(workspaceFolderParams, cancellationToken);
        }

        public static Task<Container<WorkspaceFolder>> RequestWorkspaceFolders(this IWorkspaceLanguageServer mediator, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(WorkspaceFolderParams.Instance, cancellationToken);
        }
    }
}
