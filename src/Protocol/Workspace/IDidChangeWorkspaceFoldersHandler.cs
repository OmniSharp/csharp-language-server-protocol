using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Workspace
{
    [Parallel, Method(WorkspaceNames.DidChangeWorkspaceFolders, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
    public interface IDidChangeWorkspaceFoldersHandler : IJsonRpcNotificationHandler<DidChangeWorkspaceFoldersParams>,
        ICapability<DidChangeWorkspaceFolderCapability>, IRegistration<object>
    {
    }

    public abstract class DidChangeWorkspaceFoldersHandler : IDidChangeWorkspaceFoldersHandler
    {
        private readonly object _registrationOptions;
        public DidChangeWorkspaceFoldersHandler(object registrationOptions)
        {
            _registrationOptions = registrationOptions;
        }
        public object GetRegistrationOptions() => _registrationOptions;

        public abstract Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWorkspaceFolderCapability capability) => Capability = capability;
        protected DidChangeWorkspaceFolderCapability Capability { get; private set; }
    }
}
