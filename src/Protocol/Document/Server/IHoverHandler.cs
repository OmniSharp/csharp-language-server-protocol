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
    public interface IHoverHandler : IJsonRpcRequestHandler<HoverParams, Hover>, IRegistration<HoverRegistrationOptions>, ICapability<HoverCapability> { }

    public abstract class HoverHandler : IHoverHandler
    {
        private readonly HoverRegistrationOptions _options;
        public HoverHandler(HoverRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public HoverRegistrationOptions GetRegistrationOptions() => _options;
        public abstract Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken);
        public virtual void SetCapability(HoverCapability capability) => Capability = capability;
        protected HoverCapability Capability { get; private set; }
    }

    public static class HoverHandlerExtensions
    {
        public static IDisposable OnHover(
            this ILanguageServerRegistry registry,
            Func<HoverParams, CancellationToken, Task<Hover>> handler,
            HoverRegistrationOptions registrationOptions = null,
            Action<HoverCapability> setCapability = null)
        {
            registrationOptions ??= new HoverRegistrationOptions();
            return registry.AddHandlers(new DelegatingHandler(handler, setCapability, registrationOptions));
        }

        class DelegatingHandler : HoverHandler
        {
            private readonly Func<HoverParams, CancellationToken, Task<Hover>> _handler;
            private readonly Action<HoverCapability> _setCapability;

            public DelegatingHandler(
                Func<HoverParams, CancellationToken, Task<Hover>> handler,
                Action<HoverCapability> setCapability,
                HoverRegistrationOptions registrationOptions) : base(registrationOptions)
            {
                _handler = handler;
                _setCapability = setCapability;
            }

            public override Task<Hover> Handle(HoverParams request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
            public override void SetCapability(HoverCapability capability) => _setCapability?.Invoke(capability);

        }
    }
}
