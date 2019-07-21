using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(EventNames.Breakpoint)]
    public interface IBreakpointHandler : IJsonRpcNotificationHandler<BreakpointEvent> { }

    public abstract class BreakpointHandler : IBreakpointHandler
    {
        public abstract Task<Unit> Handle(BreakpointEvent request, CancellationToken cancellationToken);
    }
    public static class BreakpointHandlerExtensions
    {
        public static IDisposable OnBreakpoint(this IDebugAdapterRegistry registry, Func<BreakpointEvent, CancellationToken, Task<Unit>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : BreakpointHandler
        {
            private readonly Func<BreakpointEvent, CancellationToken, Task<Unit>> _handler;

            public DelegatingHandler(Func<BreakpointEvent, CancellationToken, Task<Unit>> handler)
            {
                _handler = handler;
            }

            public override Task<Unit> Handle(BreakpointEvent request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
