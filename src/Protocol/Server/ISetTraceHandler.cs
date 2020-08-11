using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    [Parallel]
    [Method(GeneralNames.SetTrace, Direction.ClientToServer)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IClientLanguageClient), typeof(ILanguageClient))]
    public interface ISetTraceHandler : IJsonRpcNotificationHandler<SetTraceParams>
    {
    }

    public abstract class SetTraceHandler : ISetTraceHandler
    {
        public abstract Task<Unit> Handle(SetTraceParams request, CancellationToken cancellationToken);
    }
}
