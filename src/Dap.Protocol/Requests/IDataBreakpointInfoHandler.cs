using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.DataBreakpointInfo, Direction.ClientToServer)]
    public interface
        IDataBreakpointInfoHandler : IJsonRpcRequestHandler<DataBreakpointInfoArguments, DataBreakpointInfoResponse>
    {
    }

    public abstract class DataBreakpointInfoHandler : IDataBreakpointInfoHandler
    {
        public abstract Task<DataBreakpointInfoResponse> Handle(DataBreakpointInfoArguments request,
            CancellationToken cancellationToken);
    }

    public static class DataBreakpointInfoExtensions
    {
        public static IDisposable OnDataBreakpointInfo(this IDebugAdapterServerRegistry registry,
            Func<DataBreakpointInfoArguments, CancellationToken, Task<DataBreakpointInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.DataBreakpointInfo, RequestHandler.For(handler));
        }

        public static IDisposable OnDataBreakpointInfo(this IDebugAdapterServerRegistry registry,
            Func<DataBreakpointInfoArguments, Task<DataBreakpointInfoResponse>> handler)
        {
            return registry.AddHandler(RequestNames.DataBreakpointInfo, RequestHandler.For(handler));
        }

        public static Task<DataBreakpointInfoResponse> RequestDataBreakpointInfo(this IDebugAdapterClient mediator, DataBreakpointInfoArguments @params, CancellationToken cancellationToken = default)
        {
            return mediator.SendRequest(@params, cancellationToken);
        }
    }
}
