using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using System.Threading;
using System;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(WorkspaceNames.DidChangeWorkspaceFolders)]
    public interface IDidChangeWorkspaceFoldersHandler : IJsonRpcNotificationHandler<DidChangeWorkspaceFoldersParams>, ICapability<DidChangeWorkspaceFolderCapability>, IRegistration<object> { }

    public abstract class DidChangeWorkspaceFoldersHandler : IDidChangeWorkspaceFoldersHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DidChangeWorkspaceFolderCapability capability);
    }

    public static class DidChangeWorkspaceFoldersHandlerExtensions
    {
        public static IDisposable OnDidChangeWorkspaceFolders(
            this ILanguageServerRegistry registry,
            Func<DidChangeWorkspaceFoldersParams, CancellationToken, Task<Unit>> handler,
            Action<DidChangeWorkspaceFolderCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability));
        }

        class DelegatingHandler : DidChangeWorkspaceFoldersHandler
        {
            private readonly Func<DidChangeWorkspaceFoldersParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<DidChangeWorkspaceFolderCapability> _setCapability;

            public DelegatingHandler(
                Func<DidChangeWorkspaceFoldersParams, CancellationToken, Task<Unit>> handler,
                Action<DidChangeWorkspaceFolderCapability> setCapability) : base()
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DidChangeWorkspaceFolderCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
