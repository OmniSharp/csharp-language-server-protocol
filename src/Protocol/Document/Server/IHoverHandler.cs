using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel, Method(DocumentNames.Hover)]
    public interface IHoverHandler : IJsonRpcRequestHandler<HoverParams, Hover>, IRegistration<HoverRegistrationOptions>, ICapability<HoverClientCapabilities> { }

    public abstract class HoverHandler : IHoverHandler
    {
        private readonly HoverRegistrationOptions _options;
        public HoverHandler(HoverRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public HoverRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(HoverClientCapabilities capability) => Capability = capability;
        protected HoverClientCapabilities Capability { get; private set; }
    }

    public static class HoverHandlerExtensions
    {
        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, CancellationToken, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions = null,
            Action<HoverClientCapabilities> setCapability = null)
        {
            registrationOptions = registrationOptions ?? new HoverRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : HoverHandler
        {
            private readonly Func<HoverParams, CancellationToken, Task<Hover>> _handler;
            private readonly Action<HoverClientCapabilities> _setCapability;

            public DelegatingHandler(
                Func<HoverParams, CancellationToken, Task<Hover>> handler,
                Action<HoverClientCapabilities> setCapability,
                HoverRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(HoverClientCapabilities capability) => _setCapability?.Invoke(capability);

        }
    }
}
