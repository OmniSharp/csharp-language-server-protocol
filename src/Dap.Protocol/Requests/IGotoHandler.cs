using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Goto)]
    public interface IGotoHandler : IJsonRpcRequestHandler<GotoArguments, GotoResponse> { }

    public abstract class GotoHandler : IGotoHandler
    {
        public abstract Task<GotoResponse> Handle(GotoArguments request, CancellationToken cancellationToken);
    }

    public static class GotoHandlerExtensions
    {
        public static IDisposable OnGoto(this IDebugAdapterRegistry registry, Func<GotoArguments, CancellationToken, Task<GotoResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : GotoHandler
        {
            private readonly Func<GotoArguments, CancellationToken, Task<GotoResponse>> _handler;

            public DelegatingHandler(Func<GotoArguments, CancellationToken, Task<GotoResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<GotoResponse> Handle(GotoArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
