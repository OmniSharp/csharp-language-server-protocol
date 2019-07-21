using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Stopped)]
    public interface IStoppedHandler : IJsonRpcNotificationHandler<StoppedEvent> { }

    public abstract class StoppedHandler : IStoppedHandler
    {
        public abstract Task<Unit> Handle(StoppedEvent request, CancellationToken cancellationToken);
    }
    public static class StoppedHandlerExtensions
    {
        public static IDisposable OnStopped(this IDebugAdapterRegistry registry, Func<StoppedEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : StoppedHandler
        {
            private readonly Func<StoppedEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<StoppedEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(StoppedEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }

}
