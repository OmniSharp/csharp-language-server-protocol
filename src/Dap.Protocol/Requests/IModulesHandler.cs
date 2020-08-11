using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Parallel]
    [Method(RequestNames.Modules, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods]
    public interface IModulesHandler : IJsonRpcRequestHandler<ModulesArguments, ModulesResponse>
    {
    }

    public abstract class ModulesHandler : IModulesHandler
    {
        public abstract Task<ModulesResponse> Handle(ModulesArguments request, CancellationToken cancellationToken);
    }
}
