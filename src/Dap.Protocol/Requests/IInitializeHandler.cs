using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Initialize, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface IInitializeHandler : IJsonRpcRequestHandler<InitializeRequestArguments, InitializeResponse>
    {
    }

    public abstract class InitializeHandler : IInitializeHandler
    {
        public abstract Task<InitializeResponse> Handle(InitializeRequestArguments request,
            CancellationToken cancellationToken);
    }
}
