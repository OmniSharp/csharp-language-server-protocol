using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Scopes, Direction.ClientToServer)]
    public interface IScopesHandler : IJsonRpcRequestHandler<ScopesArguments, ScopesResponse>
    {
    }

    public abstract class ScopesHandler : IScopesHandler
    {
        public abstract Task<ScopesResponse> Handle(ScopesArguments request, CancellationToken cancellationToken);
    }

    public static class ScopesExtensions
    {
        public static IDebugAdapterServerRegistry OnScopes(this IDebugAdapterServerRegistry registry,
            Func<ScopesArguments, CancellationToken, Task<ScopesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Scopes, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnScopes(this IDebugAdapterServerRegistry registry,
            Func<ScopesArguments, Task<ScopesResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Scopes, RequestHandler.For(handler));
        }

        public static Task<ScopesResponse> RequestScopes(this IDebugAdapterClient mediator, ScopesArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
