using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals
{

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(WorkspaceNames.CodeLensRefresh, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface ICodeLensRefreshHandler : IJsonRpcRequestHandler<CodeLensRefreshParams>, ICapability<CodeLensCapability> { }

    [Obsolete(Constants.Proposal)]
    public abstract class CodeLensRefreshHandlerBase : ICodeLensRefreshHandler
    {
        protected CodeLensCapability? Capability { get; private set; }

        public abstract Task<Unit> Handle(CodeLensRefreshParams request, CancellationToken cancellationToken);
        public void SetCapability(CodeLensCapability capability) => Capability = capability;
    }
}
