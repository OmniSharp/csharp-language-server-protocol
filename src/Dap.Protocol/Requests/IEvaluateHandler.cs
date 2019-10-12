using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Evaluate)]
    public interface IEvaluateHandler : IJsonRpcRequestHandler<EvaluateArguments, EvaluateResponse> { }


    public abstract class EvaluateHandler : IEvaluateHandler
    {
        public abstract Task<EvaluateResponse> Handle(EvaluateArguments request, CancellationToken cancellationToken);
    }

    public static class EvaluateHandlerExtensions
    {
        public static IDisposable OnEvaluate(this IDebugAdapterRegistry registry, Func<EvaluateArguments, CancellationToken, Task<EvaluateResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : EvaluateHandler
        {
            private readonly Func<EvaluateArguments, CancellationToken, Task<EvaluateResponse>> _handler;

            public DelegatingHandler(Func<EvaluateArguments, CancellationToken, Task<EvaluateResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<EvaluateResponse> Handle(EvaluateArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
