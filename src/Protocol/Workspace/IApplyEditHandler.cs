using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel]
    [Method(WorkspaceNames.ApplyEdit, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface IApplyWorkspaceEditHandler : IJsonRpcRequestHandler<ApplyWorkspaceEditParams, ApplyWorkspaceEditResponse>
    {
    }

    public abstract class ApplyWorkspaceEditHandler : IApplyWorkspaceEditHandler
    {
        public abstract Task<ApplyWorkspaceEditResponse> Handle(ApplyWorkspaceEditParams request, CancellationToken cancellationToken);
    }
}
