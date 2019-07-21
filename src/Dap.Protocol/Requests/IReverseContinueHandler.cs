using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ReverseContinue)]
    public interface IReverseContinueHandler : IJsonRpcRequestHandler<ReverseContinueArguments, ReverseContinueResponse> { }

    public abstract class ReverseContinueHandler : IReverseContinueHandler
    {
        public abstract Task<ReverseContinueResponse> Handle(ReverseContinueArguments request, CancellationToken cancellationToken);
    }

    public static class ReverseContinueHandlerExtensions
    {
        public static IDisposable OnReverseContinue(this IDebugAdapterRegistry registry, Func<ReverseContinueArguments, CancellationToken, Task<ReverseContinueResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ReverseContinueHandler
        {
            private readonly Func<ReverseContinueArguments, CancellationToken, Task<ReverseContinueResponse>> _handler;

            public DelegatingHandler(Func<ReverseContinueArguments, CancellationToken, Task<ReverseContinueResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ReverseContinueResponse> Handle(ReverseContinueArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
