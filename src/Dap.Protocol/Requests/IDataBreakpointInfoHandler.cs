using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.DataBreakpointInfo)]
    public interface IDataBreakpointInfoHandler : IJsonRpcRequestHandler<DataBreakpointInfoArguments, DataBreakpointInfoResponse> { }

    public abstract class DataBreakpointInfoHandler : IDataBreakpointInfoHandler
    {
        public abstract Task<DataBreakpointInfoResponse> Handle(DataBreakpointInfoArguments request, CancellationToken cancellationToken);
    }

    public static class DataBreakpointInfoHandlerExtensions
    {
        public static IDisposable OnDataBreakpointInfo(this IDebugAdapterRegistry registry, Func<DataBreakpointInfoArguments, CancellationToken, Task<DataBreakpointInfoResponse>> handler)
        {
            return registry.AddHandlers(new DelegatingHandler(handler));
        }

        class DelegatingHandler : DataBreakpointInfoHandler
        {
            private readonly Func<DataBreakpointInfoArguments, CancellationToken, Task<DataBreakpointInfoResponse>> _handler;

            public DelegatingHandler(Func<DataBreakpointInfoArguments, CancellationToken, Task<DataBreakpointInfoResponse>> handler)
            {
                _handler = handler;
            }

            public override Task<DataBreakpointInfoResponse> Handle(DataBreakpointInfoArguments request, CancellationToken cancellationToken) => _handler.Invoke(request, cancellationToken);
        }
    }
}
