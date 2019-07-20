using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.RestartFrame)]
    public interface IRestartFrameHandler : IJsonRpcRequestHandler<RestartFrameArguments, RestartFrameResponse> { }

    public abstract class RestartFrameHandler : IRestartFrameHandler
    {
        public abstract Task<RestartFrameResponse> Handle(RestartFrameArguments request, CancellationToken cancellationToken);
    }

    public static class RestartFrameHandlerExtensions
    {
        public static IDisposable OnRestartFrame(this IDebugAdapterRegistry registry, Func<RestartFrameArguments, CancellationToken, Task<RestartFrameResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : RestartFrameHandler
        {
            private readonly Func<RestartFrameArguments, CancellationToken, Task<RestartFrameResponse>> _handler;

            public DelegatingHandler(Func<RestartFrameArguments, CancellationToken, Task<RestartFrameResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<RestartFrameResponse> Handle(RestartFrameArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
