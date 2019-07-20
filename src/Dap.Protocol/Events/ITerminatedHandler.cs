using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Terminated)]
    public interface ITerminatedHandler : IJsonRpcNotificationHandler<TerminatedEvent> { }

    public abstract class TerminatedHandler : ITerminatedHandler
    {
        public abstract Task<Unit> Handle(TerminatedEvent request, CancellationToken cancellationToken);
    }
    public static class TerminatedHandlerExtensions
    {
        public static IDisposable OnTerminated(this IDebugAdapterRegistry registry, Func<TerminatedEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : TerminatedHandler
        {
            private readonly Func<TerminatedEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<TerminatedEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(TerminatedEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
