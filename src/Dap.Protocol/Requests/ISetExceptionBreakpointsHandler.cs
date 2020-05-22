using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetExceptionBreakpoints, Direction.ClientToServer)]
    public interface ISetExceptionBreakpointsHandler : IJsonRpcRequestHandler<SetExceptionBreakpointsArguments,
        SetExceptionBreakpointsResponse>
    {
    }

    public abstract class SetExceptionBreakpointsHandler : ISetExceptionBreakpointsHandler
    {
        public abstract Task<SetExceptionBreakpointsResponse> Handle(SetExceptionBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetExceptionBreakpointsExtensions
    {
        public static IDisposable OnSetExceptionBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetExceptionBreakpointsArguments, CancellationToken, Task<SetExceptionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExceptionBreakpoints, RequestHandler.For(handler));
        }

        public static IDisposable OnSetExceptionBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetExceptionBreakpointsArguments, Task<SetExceptionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetExceptionBreakpoints, RequestHandler.For(handler));
        }

        public static Task<SetExceptionBreakpointsResponse> RequestSetExceptionBreakpoints(this IDebugAdapterClient mediator, SetExceptionBreakpointsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
