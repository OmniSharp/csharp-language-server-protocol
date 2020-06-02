using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.GotoTargets, Direction.ClientToServer)]
    public interface IGotoTargetsHandler : IJsonRpcRequestHandler<GotoTargetsArguments, GotoTargetsResponse>
    {
    }


    public abstract class GotoTargetsHandler : IGotoTargetsHandler
    {
        public abstract Task<GotoTargetsResponse> Handle(GotoTargetsArguments request,
            CancellationToken cancellationToken);
    }

    public static class GotoTargetsExtensions
    {
        public static IDebugAdapterServerRegistry OnGotoTargets(this IDebugAdapterServerRegistry registry,
            Func<GotoTargetsArguments, CancellationToken, Task<GotoTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.GotoTargets, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnGotoTargets(this IDebugAdapterServerRegistry registry,
            Func<GotoTargetsArguments, Task<GotoTargetsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.GotoTargets, RequestHandler.For(handler));
        }

        public static Task<GotoTargetsResponse> RequestGotoTargets(this IDebugAdapterClient mediator, GotoTargetsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
