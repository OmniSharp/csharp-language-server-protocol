using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Disconnect, Direction.ClientToServer)]
    public interface IDisconnectHandler : IJsonRpcRequestHandler<DisconnectArguments, DisconnectResponse>
    {
    }

    public abstract class DisconnectHandler : IDisconnectHandler
    {
        public abstract Task<DisconnectResponse> Handle(DisconnectArguments request,
            CancellationToken cancellationToken);
    }

    public static class DisconnectExtensions
    {
        public static IDisposable OnDisconnect(this IDebugAdapterServerRegistry registry,
            Func<DisconnectArguments, CancellationToken, Task<DisconnectResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disconnect, RequestHandler.For(handler));
        }

        public static IDisposable OnDisconnect(this IDebugAdapterServerRegistry registry,
            Func<DisconnectArguments, Task<DisconnectResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Disconnect, RequestHandler.For(handler));
        }

        public static Task<DisconnectResponse> RequestDisconnect(this IDebugAdapterClient mediator, DisconnectArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
