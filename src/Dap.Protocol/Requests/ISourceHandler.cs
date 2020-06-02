using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Source, Direction.ClientToServer)]
    public interface ISourceHandler : IJsonRpcRequestHandler<SourceArguments, SourceResponse>
    {
    }

    public abstract class SourceHandler : ISourceHandler
    {
        public abstract Task<SourceResponse> Handle(SourceArguments request, CancellationToken cancellationToken);
    }

    public static class SourceExtensions
    {
        public static IDebugAdapterServerRegistry OnSource(this IDebugAdapterServerRegistry registry,
            Func<SourceArguments, CancellationToken, Task<SourceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Source, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnSource(this IDebugAdapterServerRegistry registry,
            Func<SourceArguments, Task<SourceResponse>> handler)
        {
            return registry.AddHandler(RequestNames.Source, RequestHandler.For(handler));
        }

        public static Task<SourceResponse> RequestSource(this IDebugAdapterClient mediator, SourceArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
