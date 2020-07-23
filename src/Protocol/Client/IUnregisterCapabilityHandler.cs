using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(ClientNames.UnregisterCapability, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
    public interface IUnregisterCapabilityHandler : IJsonRpcRequestHandler<UnregistrationParams> { }

    public abstract class UnregisterCapabilityHandler : IUnregisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(UnregistrationParams request, CancellationToken cancellationToken);
    }
}
