using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Continue)]
    public interface IContinueHandler : IJsonRpcRequestHandler<ContinueArguments, ContinueResponse> { }

    public abstract class ContinueHandler : IContinueHandler
    {
        public abstract Task<ContinueResponse> Handle(ContinueArguments request, CancellationToken cancellationToken);
    }

    public static class ContinueHandlerExtensions
    {
        public static IDisposable OnContinue(this IDebugAdapterRegistry registry, Func<ContinueArguments, CancellationToken, Task<ContinueResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : ContinueHandler
        {
            private readonly Func<ContinueArguments, CancellationToken, Task<ContinueResponse>> _handler;

            public DelegatingHandler(Func<ContinueArguments, CancellationToken, Task<ContinueResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<ContinueResponse> Handle(ContinueArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
