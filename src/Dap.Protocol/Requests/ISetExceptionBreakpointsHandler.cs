using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.SetExceptionBreakpoints)]
    public interface ISetExceptionBreakpointsHandler : IJsonRpcRequestHandler<SetExceptionBreakpointsArguments, SetExceptionBreakpointsResponse> { }

    public abstract class SetExceptionBreakpointsHandler : ISetExceptionBreakpointsHandler
    {
        public abstract Task<SetExceptionBreakpointsResponse> Handle(SetExceptionBreakpointsArguments request, CancellationToken cancellationToken);
    }

    public static class SetExceptionBreakpointsHandlerExtensions
    {
        public static IDisposable OnSetExceptionBreakpoints(this IDebugAdapterRegistry registry, Func<SetExceptionBreakpointsArguments, CancellationToken, Task<SetExceptionBreakpointsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : SetExceptionBreakpointsHandler
        {
            private readonly Func<SetExceptionBreakpointsArguments, CancellationToken, Task<SetExceptionBreakpointsResponse>> _handler;

            public DelegatingHandler(Func<SetExceptionBreakpointsArguments, CancellationToken, Task<SetExceptionBreakpointsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<SetExceptionBreakpointsResponse> Handle(SetExceptionBreakpointsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
