using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel, Method(RequestNames.ConfigurationDone, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods]
    public interface
        IConfigurationDoneHandler : IJsonRpcRequestHandler<ConfigurationDoneArguments, ConfigurationDoneResponse>
    {
    }

    public abstract class ConfigurationDoneHandler : IConfigurationDoneHandler
    {
        public abstract Task<ConfigurationDoneResponse> Handle(ConfigurationDoneArguments request,
            CancellationToken cancellationToken);
    }
}
