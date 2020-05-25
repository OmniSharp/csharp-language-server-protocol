using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.StackTrace, Direction.ClientToServer)]
    public interface IStackTraceHandler : IJsonRpcRequestHandler<StackTraceArguments, StackTraceResponse>
    {
    }

    public abstract class StackTraceHandler : IStackTraceHandler
    {
        public abstract Task<StackTraceResponse> Handle(StackTraceArguments request,
            CancellationToken cancellationToken);
    }

    public static class StackTraceExtensions
    {
        public static IDebugAdapterServerRegistry OnStackTrace(this IDebugAdapterServerRegistry registry,
            Func<StackTraceArguments, CancellationToken, Task<StackTraceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StackTrace, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnStackTrace(this IDebugAdapterServerRegistry registry,
            Func<StackTraceArguments, Task<StackTraceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.StackTrace, RequestHandler.For(handler));
        }

        public static Task<StackTraceResponse> RequestStackTrace(this IDebugAdapterClient mediator, StackTraceArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
