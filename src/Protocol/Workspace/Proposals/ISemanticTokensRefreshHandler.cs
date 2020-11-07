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
    [Method(WorkspaceNames.SemanticTokensRefresh, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface ISemanticTokensRefreshHandler : IJsonRpcRequestHandler<SemanticTokensRefreshParams>, ICapability<SemanticTokensCapability> { }

    [Obsolete(Constants.Proposal)]
    public abstract class SemanticTokensRefreshHandlerBase : ISemanticTokensRefreshHandler
    {
        protected SemanticTokensCapability? Capability { get; private set; }

        public abstract Task<Unit> Handle(SemanticTokensRefreshParams request, CancellationToken cancellationToken);
        public void SetCapability(SemanticTokensCapability capability) => Capability = capability;
    }

    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(WorkspaceNames.CodeLensRefresh, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
    public interface ICodeLensRefreshHandler : IJsonRpcRequestHandler<CodeLensRefreshParams>, ICapability<CodeLensCapability> { }
}
