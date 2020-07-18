using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.WorkspaceSymbol, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IWorkspaceSymbolsHandler : IJsonRpcRequestHandler<WorkspaceSymbolParams, Container<SymbolInformation>>, ICapability<WorkspaceSymbolCapability>, IRegistration<WorkspaceSymbolRegistrationOptions> { }

    public abstract class WorkspaceSymbolsHandler : IWorkspaceSymbolsHandler
    {
        protected WorkspaceSymbolCapability Capability { get; private set; }
        private readonly WorkspaceSymbolRegistrationOptions _options;

        public WorkspaceSymbolsHandler(WorkspaceSymbolRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public WorkspaceSymbolRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Container<SymbolInformation>> Handle(WorkspaceSymbolParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(WorkspaceSymbolCapability capability) => Capability = capability;
    }
}
