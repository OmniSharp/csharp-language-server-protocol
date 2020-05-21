using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Next, Direction.ClientToServer)]
    public interface INextHandler : IJsonRpcRequestHandler<NextArguments, NextResponse>
    {
    }

    public abstract class NextHandler : INextHandler
    {
        public abstract Task<NextResponse> Handle(NextArguments request, CancellationToken cancellationToken);
    }

    public static class NextExtensions
    {
        public static IDisposable OnNext(this IDebugAdapterServerRegistry registry,
            Func<NextArguments, CancellationToken, Task<NextResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Next, RequestHandler.For(handler));
        }

        public static IDisposable OnNext(this IDebugAdapterServerRegistry registry,
            Func<NextArguments, Task<NextResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Next, RequestHandler.For(handler));
        }

        public static Task<NextResponse> RequestNext(this IDebugAdapterClient mediator, NextArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
