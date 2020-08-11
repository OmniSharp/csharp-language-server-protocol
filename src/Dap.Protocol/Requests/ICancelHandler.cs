using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    /// <summary>
    /// DAP is kind of silly....
    /// Cancellation is for requests and progress tokens... hopefully if isn't ever expanded any further... because that would be fun.
    /// </summary>
    [Parallel]
    [Method(RequestNames.Cancel, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface ICancelHandler : IJsonRpcRequestHandler<CancelArguments, CancelResponse>
    {
    }

    public abstract class CancelHandlerBase : ICancelHandler
    {
        public abstract Task<CancelResponse> Handle(CancelArguments request, CancellationToken cancellationToken);
    }
}
