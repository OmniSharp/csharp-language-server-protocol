using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.ExceptionInfo, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IExceptionInfoHandler : IJsonRpcRequestHandler<ExceptionInfoArguments, ExceptionInfoResponse>
    {
    }

    public abstract class ExceptionInfoHandler : IExceptionInfoHandler
    {
        public abstract Task<ExceptionInfoResponse> Handle(
            ExceptionInfoArguments request,
            CancellationToken cancellationToken
        );
    }
}
