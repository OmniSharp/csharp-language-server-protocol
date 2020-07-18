using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Window
{
    [Parallel, Method(WindowNames.TelemetryEvent, Direction.ServerToClient)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface ITelemetryEventHandler : IJsonRpcNotificationHandler<TelemetryEventParams> { }

    public abstract class TelemetryEventHandler : ITelemetryEventHandler
    {
        public abstract Task<Unit> Handle(TelemetryEventParams request, CancellationToken cancellationToken);
    }
}
