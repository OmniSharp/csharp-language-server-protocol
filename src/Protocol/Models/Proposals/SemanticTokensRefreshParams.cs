using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [Parallel]
    [Method(WorkspaceNames.SemanticTokensRefresh, Direction.ServerToClient)]
    public class SemanticTokensRefreshParams : IRequest { }
}
