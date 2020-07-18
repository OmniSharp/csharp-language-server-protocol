using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.Launch, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface ILaunchHandler : IJsonRpcRequestHandler<LaunchRequestArguments, LaunchResponse>
    {
    }

    public abstract class LaunchHandler : ILaunchHandler
    {
        public abstract Task<LaunchResponse>
            Handle(LaunchRequestArguments request, CancellationToken cancellationToken);
    }
}
