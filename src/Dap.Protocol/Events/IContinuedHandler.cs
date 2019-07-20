using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Continued)]
    public interface IContinuedHandler : IJsonRpcNotificationHandler<ContinuedEvent> { }

    public abstract class ContinuedHandler : IContinuedHandler
    {
        public abstract Task<Unit> Handle(ContinuedEvent request, CancellationToken cancellationToken);
    }
    public static class ContinuedHandlerExtensions
    {
        public static IDisposable OnContinued(this IDebugAdapterRegistry registry, Func<ContinuedEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ContinuedHandler
        {
            private readonly Func<ContinuedEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<ContinuedEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(ContinuedEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
