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
    [Method(WorkspaceNames.DidChangeConfiguration, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
    public interface IDidChangeConfigurationHandler : IJsonRpcNotificationHandler<DidChangeConfigurationParams>,
                                                      IRegistration<object>, // TODO: Remove this in the future
                                                      ICapability<DidChangeConfigurationCapability>
    {
    }

    public abstract class DidChangeConfigurationHandler : IDidChangeConfigurationHandler
    {
        public abstract Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeConfigurationCapability capability) => Capability = capability;
        protected DidChangeConfigurationCapability Capability { get; private set; } = null!;
        public object GetRegistrationOptions() => new object();
    }
}
