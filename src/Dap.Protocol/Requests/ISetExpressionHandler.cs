using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetExpression, Direction.ClientToServer)]
    public interface ISetExpressionHandler : IJsonRpcRequestHandler<SetExpressionArguments, SetExpressionResponse>
    {
    }

    public abstract class SetExpressionHandler : ISetExpressionHandler
    {
        public abstract Task<SetExpressionResponse> Handle(SetExpressionArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetExpressionExtensions
    {
        public static IDisposable OnSetExpression(this IDebugAdapterServerRegistry registry,
            Func<SetExpressionArguments, CancellationToken, Task<SetExpressionResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExpression, RequestHandler.For(handler));
        }

        public static IDisposable OnSetExpression(this IDebugAdapterServerRegistry registry,
            Func<SetExpressionArguments, Task<SetExpressionResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExpression, RequestHandler.For(handler));
        }

        public static Task<SetExpressionResponse> RequestSetExpression(this IDebugAdapterClient mediator, SetExpressionArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
