using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(WorkspaceNames.DidChangeWatchedFiles)]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<object>, ICapability<DidChangeWatchedFilesCapability> { }

    public abstract class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        public object GetRegistrationOptions() => new object();
        public abstract Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken);
        public abstract void SetCapability(DidChangeWatchedFilesCapability capability);
    }

    public static class DidChangeWatchedFilesHandlerExtensions
    {
        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> handler,
            Action<DidChangeWatchedFilesCapability> setCapability = null)
        {
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability));
        }

        class DelegatingHandler : DidChangeWatchedFilesHandler
        {
            private readonly Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<DidChangeWatchedFilesCapability> _setCapability;

            public DelegatingHandler(
                Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> handler,
                Action<DidChangeWatchedFilesCapability> setCapability) : base()
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DidChangeWatchedFilesCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
