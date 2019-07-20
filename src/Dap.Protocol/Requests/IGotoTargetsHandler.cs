using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Parallel, Method(RequestNames.GotoTargets)]
    public interface IGotoTargetsHandler : IJsonRpcRequestHandler<GotoTargetsArguments, GotoTargetsResponse> { }


    public abstract class GotoTargetsHandler : IGotoTargetsHandler
    {
        public abstract Task<GotoTargetsResponse> Handle(GotoTargetsArguments request, CancellationToken cancellationToken);
    }

    public static class GotoTargetsHandlerExtensions
    {
        public static IDisposable OnGotoTargets(this IDebugAdapterRegistry registry, Func<GotoTargetsArguments, CancellationToken, Task<GotoTargetsResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : GotoTargetsHandler
        {
            private readonly Func<GotoTargetsArguments, CancellationToken, Task<GotoTargetsResponse>> _handler;

            public DelegatingHandler(Func<GotoTargetsArguments, CancellationToken, Task<GotoTargetsResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<GotoTargetsResponse> Handle(GotoTargetsArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
