using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.RestartFrame, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IRestartFrameHandler : IJsonRpcRequestHandler<RestartFrameArguments, RestartFrameResponse>
    {
    }

    public abstract class RestartFrameHandler : IRestartFrameHandler
    {
        public abstract Task<RestartFrameResponse> Handle(RestartFrameArguments request,
            CancellationToken cancellationToken);
    }
}
