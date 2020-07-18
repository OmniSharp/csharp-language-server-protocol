using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Attach, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IAttachHandler : IJsonRpcRequestHandler<AttachRequestArguments, AttachResponse> { }

    public abstract class AttachHandler : IAttachHandler
    {
        public abstract Task<AttachResponse> Handle(AttachRequestArguments request, CancellationToken cancellationToken);
    }
}
