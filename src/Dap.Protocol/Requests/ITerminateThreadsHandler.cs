using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.TerminateThreads, Direction.ClientToServer)]
    public interface
        ITerminateThreadsHandler : IJsonRpcRequestHandler<TerminateThreadsArguments, TerminateThreadsResponse>
    {
    }

    public abstract class TerminateThreadsHandler : ITerminateThreadsHandler
    {
        public abstract Task<TerminateThreadsResponse> Handle(TerminateThreadsArguments request,
            CancellationToken cancellationToken);
    }

    public static class TerminateThreadsExtensions
    {
        public static IDebugAdapterServerRegistry OnTerminateThreads(this IDebugAdapterServerRegistry registry,
            Func<TerminateThreadsArguments, CancellationToken, Task<TerminateThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.TerminateThreads, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnTerminateThreads(this IDebugAdapterServerRegistry registry,
            Func<TerminateThreadsArguments, Task<TerminateThreadsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.TerminateThreads, RequestHandler.For(handler));
        }

        public static Task<TerminateThreadsResponse> RequestTerminateThreads(this IDebugAdapterClient mediator, TerminateThreadsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
