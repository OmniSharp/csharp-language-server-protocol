using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Attach, Direction.ClientToServer)]
    [GenerateHandlerMethods(AllowDerivedRequests = true)]
    [GenerateRequestMethods]
    public interface IAttachHandler<in T> : IJsonRpcRequestHandler<T, AttachResponse> where T : AttachRequestArguments
    {
    }

    public interface IAttachHandler : IAttachHandler<AttachRequestArguments>
    {
    }

    public abstract class AttachHandlerBase<T> : IAttachHandler<T> where T : AttachRequestArguments
    {
        public abstract Task<AttachResponse> Handle(T request, CancellationToken cancellationToken);
    }

    public abstract class AttachHandler : AttachHandlerBase<AttachRequestArguments>, IAttachHandler
    {
    }
}
