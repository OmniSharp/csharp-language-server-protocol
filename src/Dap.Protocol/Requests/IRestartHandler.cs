using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Restart)]
    public interface IRestartHandler : IJsonRpcRequestHandler<RestartArguments, RestartResponse> { }

    public abstract class RestartHandler : IRestartHandler
    {
        public abstract Task<RestartResponse> Handle(RestartArguments request, CancellationToken cancellationToken);
    }

    public static class RestartHandlerExtensions
    {
        public static IDisposable OnRestart(this IDebugAdapterRegistry registry, Func<RestartArguments, CancellationToken, Task<RestartResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : RestartHandler
        {
            private readonly Func<RestartArguments, CancellationToken, Task<RestartResponse>> _handler;

            public DelegatingHandler(Func<RestartArguments, CancellationToken, Task<RestartResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<RestartResponse> Handle(RestartArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
