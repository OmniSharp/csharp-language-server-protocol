using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel]
    [Method(WorkspaceNames.DidChangeWatchedFiles, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<DidChangeWatchedFilesRegistrationOptions>,
                                                     ICapability<DidChangeWatchedFilesCapability>
    {
    }

    public abstract class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private readonly DidChangeWatchedFilesRegistrationOptions _options;
        public DidChangeWatchedFilesHandler(DidChangeWatchedFilesRegistrationOptions registrationOptions) => _options = registrationOptions;

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWatchedFilesCapability capability) => Capability = capability;
        protected DidChangeWatchedFilesCapability Capability { get; private set; }
    }
}
