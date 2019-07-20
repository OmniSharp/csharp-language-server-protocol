using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Capabilities)]
    public interface ICapabilitiesHandler : IJsonRpcNotificationHandler<CapabilitiesEvent> { }

    public abstract class CapabilitiesHandler : ICapabilitiesHandler
    {
        public abstract Task<Unit> Handle(CapabilitiesEvent request, CancellationToken cancellationToken);
    }
    public static class CapabilitiesHandlerExtensions
    {
        public static IDisposable OnCapabilities(this IDebugAdapterRegistry registry, Func<CapabilitiesEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : CapabilitiesHandler
        {
            private readonly Func<CapabilitiesEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<CapabilitiesEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(CapabilitiesEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
