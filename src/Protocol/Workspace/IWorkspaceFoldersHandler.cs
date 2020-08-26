using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel]
    [Method(WorkspaceNames.WorkspaceFolders, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface IWorkspaceFoldersHandler : IJsonRpcRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>?>
    {
    }

    public abstract class WorkspaceFoldersHandler : IWorkspaceFoldersHandler
    {
        public abstract Task<Container<WorkspaceFolder>?> Handle(WorkspaceFolderParams request, CancellationToken cancellationToken);
    }
}
