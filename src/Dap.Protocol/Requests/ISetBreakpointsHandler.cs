using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetBreakpoints, Direction.ClientToServer)]
    public interface ISetBreakpointsHandler : IJsonRpcRequestHandler<SetBreakpointsArguments, SetBreakpointsResponse>
    {
    }

    public abstract class SetBreakpointsHandler : ISetBreakpointsHandler
    {
        public abstract Task<SetBreakpointsResponse> Handle(SetBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetBreakpointsExtensions
    {
        public static IDisposable OnSetBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetBreakpointsArguments, CancellationToken, Task<SetBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetBreakpointsArguments, Task<SetBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetBreakpoints, RequestHandler.For(handler));
        }

        public static Task<SetBreakpointsResponse> RequestSetBreakpoints(this IDebugAdapterClient mediator, SetBreakpointsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
