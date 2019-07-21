using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StackTrace)]
    public interface IStackTraceHandler : IJsonRpcRequestHandler<StackTraceArguments, StackTraceResponse> { }

    public abstract class StackTraceHandler : IStackTraceHandler
    {
        public abstract Task<StackTraceResponse> Handle(StackTraceArguments request, CancellationToken cancellationToken);
    }

    public static class StackTraceHandlerExtensions
    {
        public static IDisposable OnStackTrace(this IDebugAdapterRegistry registry, Func<StackTraceArguments, CancellationToken, Task<StackTraceResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : StackTraceHandler
        {
            private readonly Func<StackTraceArguments, CancellationToken, Task<StackTraceResponse>> _handler;

            public DelegatingHandler(Func<StackTraceArguments, CancellationToken, Task<StackTraceResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<StackTraceResponse> Handle(StackTraceArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
