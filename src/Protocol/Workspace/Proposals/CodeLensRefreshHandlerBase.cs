using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals
{
    [Obsolete(Constants.Proposal)]
    public abstract class CodeLensRefreshHandlerBase : ICodeLensRefreshHandler
    {
        protected CodeLensCapability? Capability { get; private set; }

        public abstract Task<Unit> Handle(CodeLensRefreshParams request, CancellationToken cancellationToken);
        public void SetCapability(CodeLensCapability capability) => Capability = capability;
    }
}
