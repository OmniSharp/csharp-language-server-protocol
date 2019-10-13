using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Serial, Method(DocumentNames.Rename)]
    public interface IRenameHandler : IJsonRpcRequestHandler<RenameParams, WorkspaceEdit>, IRegistration<RenameRegistrationOptions>, ICapability<RenameClientCapabilities> { }

    public abstract class RenameHandler : IRenameHandler
    {
        private readonly RenameRegistrationOptions _options;
        public RenameHandler(RenameRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public RenameRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<WorkspaceEdit> Handle(RenameParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(RenameClientCapabilities capability) => Capability = capability;
        protected RenameClientCapabilities Capability { get; private set; }
    }

    public static class RenameHandlerExtensions
    {
        public static IDisposable OnRename(
            this ILanguageServerRegistry registry,
            Func<RenameParams, CancellationToken, Task<WorkspaceEdit>> handler,
            RenameRegistrationOptions registrationOptions = null,
            Action<RenameClientCapabilities> setCapability = null)
        {
            registrationOptions ??= new RenameRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : RenameHandler
        {
            private readonly Func<RenameParams, CancellationToken, Task<WorkspaceEdit>> _handler;
            private readonly Action<RenameClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<RenameParams, CancellationToken, Task<WorkspaceEdit>> handler,
                Action<RenameClientCapabilities> setCapability,
                RenameRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<WorkspaceEdit> Handle(RenameParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(RenameClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
