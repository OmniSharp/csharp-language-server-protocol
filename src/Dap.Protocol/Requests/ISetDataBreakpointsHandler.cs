using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetDataBreakpoints, Direction.ClientToServer)]
    public interface
        ISetDataBreakpointsHandler : IJsonRpcRequestHandler<SetDataBreakpointsArguments, SetDataBreakpointsResponse>
    {
    }

    public abstract class SetDataBreakpointsHandler : ISetDataBreakpointsHandler
    {
        public abstract Task<SetDataBreakpointsResponse> Handle(SetDataBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetDataBreakpointsExtensions
    {
        public static IDisposable OnSetDataBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetDataBreakpointsArguments, CancellationToken, Task<SetDataBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetDataBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetDataBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetDataBreakpointsArguments, Task<SetDataBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetDataBreakpoints, RequestHandler.For(handler));
        }

        public static Task<SetDataBreakpointsResponse> RequestSetDataBreakpoints(this IDebugAdapterClient mediator, SetDataBreakpointsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
