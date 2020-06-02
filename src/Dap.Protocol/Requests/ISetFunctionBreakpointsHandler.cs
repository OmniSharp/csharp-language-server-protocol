using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.SetFunctionBreakpoints, Direction.ClientToServer)]
    public interface
        ISetFunctionBreakpointsHandler : IJsonRpcRequestHandler<SetFunctionBreakpointsArguments,
            SetFunctionBreakpointsResponse>
    {
    }

    public abstract class SetFunctionBreakpointsHandler : ISetFunctionBreakpointsHandler
    {
        public abstract Task<SetFunctionBreakpointsResponse> Handle(SetFunctionBreakpointsArguments request,
            CancellationToken cancellationToken);
    }

    public static class SetFunctionBreakpointsExtensions
    {
        public static IDebugAdapterServerRegistry OnSetFunctionBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetFunctionBreakpointsArguments, CancellationToken, Task<SetFunctionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetFunctionBreakpoints, RequestHandler.For(handler));
        }

        public static IDebugAdapterServerRegistry OnSetFunctionBreakpoints(this IDebugAdapterServerRegistry registry,
            Func<SetFunctionBreakpointsArguments, Task<SetFunctionBreakpointsResponse>> handler)
        {
            return registry.AddHandler(RequestNames.SetFunctionBreakpoints, RequestHandler.For(handler));
        }

        public static Task<SetFunctionBreakpointsResponse> RequestSetFunctionBreakpoints(this IDebugAdapterClient mediator, SetFunctionBreakpointsArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
