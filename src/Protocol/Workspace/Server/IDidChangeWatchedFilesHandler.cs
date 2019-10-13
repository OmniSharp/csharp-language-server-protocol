using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(WorkspaceNames.DidChangeWatchedFiles)]
    public interface IDidChangeWatchedFilesHandler : IJsonRpcNotificationHandler<DidChangeWatchedFilesParams>, IRegistration<DidChangeWatchedFilesRegistrationOptions>, ICapability<DidChangeWatchedFilesClientCapabilities> { }

    public abstract class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private readonly DidChangeWatchedFilesRegistrationOptions _options;
        public DidChangeWatchedFilesHandler(DidChangeWatchedFilesRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(DidChangeWatchedFilesClientCapabilities capability) => Capability = capability;
        protected DidChangeWatchedFilesClientCapabilities Capability { get; private set; }
    }

    public static class DidChangeWatchedFilesHandlerExtensions
    {
        public static IDisposable OnDidChangeWatchedFiles(
            this ILanguageServerRegistry registry,
            Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> handler,
            Action<DidChangeWatchedFilesClientCapabilities> setCapability = null,
            DidChangeWatchedFilesRegistrationOptions registrationOptions = null)
        {
            registrationOptions = registrationOptions ?? new DidChangeWatchedFilesRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : DidChangeWatchedFilesHandler
        {
            private readonly Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> _handler;
            private readonly Action<DidChangeWatchedFilesClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<DidChangeWatchedFilesParams, CancellationToken, Task<Unit>> handler,
                Action<DidChangeWatchedFilesClientCapabilities> setCapability,
                DidChangeWatchedFilesRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(DidChangeWatchedFilesClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
