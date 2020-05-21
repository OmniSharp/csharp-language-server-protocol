using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Evaluate, Direction.ClientToServer)]
    public interface IEvaluateHandler : IJsonRpcRequestHandler<EvaluateArguments, EvaluateResponse>
    {
    }


    public abstract class EvaluateHandler : IEvaluateHandler
    {
        public abstract Task<EvaluateResponse> Handle(EvaluateArguments request, CancellationToken cancellationToken);
    }

    public static class EvaluateExtensions
    {
        public static IDisposable OnEvaluate(this IDebugAdapterServerRegistry registry,
            Func<EvaluateArguments, CancellationToken, Task<EvaluateResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Evaluate, RequestHandler.For(handler));
        }

        public static IDisposable OnEvaluate(this IDebugAdapterServerRegistry registry,
            Func<EvaluateArguments, Task<EvaluateResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Evaluate, RequestHandler.For(handler));
        }

        public static Task<EvaluateResponse> RequestEvaluate(this IDebugAdapterClient mediator, EvaluateArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
