using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Parallel]
    [Method(GeneralNames.LogTrace, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IClientLanguageServer), typeof(ILanguageServer))]
    public interface ILogTraceHandler : IJsonRpcNotificationHandler<LogTraceParams>
    {
    }

    public abstract class LogTraceHandler : ILogTraceHandler
    {
        public abstract Task<Unit> Handle(LogTraceParams request, CancellationToken cancellationToken);
    }
}
