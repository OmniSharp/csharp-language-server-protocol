using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Disconnect)]
    public interface IDisconnectHandler : IJsonRpcRequestHandler<DisconnectArguments, DisconnectResponse> { }

    public abstract class DisconnectHandler : IDisconnectHandler
    {
        public abstract Task<DisconnectResponse> Handle(DisconnectArguments request, CancellationToken cancellationToken);
    }

    public static class DisconnectHandlerExtensions
    {
        public static IDisposable OnDisconnect(this IDebugAdapterRegistry registry, Func<DisconnectArguments, CancellationToken, Task<DisconnectResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : DisconnectHandler
        {
            private readonly Func<DisconnectArguments, CancellationToken, Task<DisconnectResponse>> _handler;

            public DelegatingHandler(Func<DisconnectArguments, CancellationToken, Task<DisconnectResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<DisconnectResponse> Handle(DisconnectArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
