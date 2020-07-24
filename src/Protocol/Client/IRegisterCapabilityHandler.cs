using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel, Method(ClientNames.RegisterCapability, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
    public interface IRegisterCapabilityHandler : IJsonRpcRequestHandler<RegistrationParams>
    {
    }

    public abstract class RegisterCapabilityHandler : IRegisterCapabilityHandler
    {
        public abstract Task<Unit> Handle(RegistrationParams request, CancellationToken cancellationToken);
    }
}
