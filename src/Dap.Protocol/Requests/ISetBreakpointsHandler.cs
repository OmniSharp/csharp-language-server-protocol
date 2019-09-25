using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetBreakpoints)]
    public interface ISetBreakpointsHandler : IJsonRpcRequestHandler<SetBreakpointsArguments, SetBreakpointsResponse> { }

    public abstract class SetBreakpointsHandler : ISetBreakpointsHandler
    {
        public abstract Task<SetBreakpointsResponse> Handle(SetBreakpointsArguments request, CancellationToken cancellationToken);
    }

    public static class SetBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetBreakpoints(this IDebugAdapterRegistry registry, Func<SetBreakpointsArguments, CancellationToken, Task<SetBreakpointsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : SetBreakpointsHandler
        {
            private readonly Func<SetBreakpointsArguments, CancellationToken, Task<SetBreakpointsResponse>> _handler;

            public DelegatingHandler(Func<SetBreakpointsArguments, CancellationToken, Task<SetBreakpointsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SetBreakpointsResponse> Handle(SetBreakpointsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
