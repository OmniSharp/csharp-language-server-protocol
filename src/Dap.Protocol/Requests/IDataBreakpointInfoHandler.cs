using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.DataBreakpointInfo, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface
        IDataBreakpointInfoHandler : IJsonRpcRequestHandler<DataBreakpointInfoArguments, DataBreakpointInfoResponse>
    {
    }

    public abstract class DataBreakpointInfoHandler : IDataBreakpointInfoHandler
    {
        public abstract Task<DataBreakpointInfoResponse> Handle(
            DataBreakpointInfoArguments request,
            CancellationToken cancellationToken
        );
    }
}
