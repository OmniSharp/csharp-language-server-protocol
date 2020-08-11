using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.General
{
    [Serial]
    [Method(GeneralNames.Initialized, Direction.ClientToServer)]
    [GenerateHandlerMethods(typeof(ILanguageServerRegistry), MethodName = "OnLanguageProtocolInitialized")]
    [GenerateRequestMethods(typeof(ILanguageClient), MethodName = "SendLanguageProtocolInitialized")]
    public interface ILanguageProtocolInitializedHandler : IJsonRpcNotificationHandler<InitializedParams>
    {
    }

    public abstract class LanguageProtocolInitializedHandler : ILanguageProtocolInitializedHandler
    {
        public abstract Task<Unit> Handle(InitializedParams request, CancellationToken cancellationToken);
    }
}
