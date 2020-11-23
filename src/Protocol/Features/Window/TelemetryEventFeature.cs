using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{

    namespace Models
    {
        [Parallel]
        [Method(WindowNames.TelemetryEvent, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window"), GenerateHandlerMethods, GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
        public class TelemetryEventParams : IRequest
        {
            [JsonExtensionData] public IDictionary<string, JToken> Data { get; set; } = new Dictionary<string, JToken>();
        }
    }

    namespace Window
    {
    }
    [Parallel]
    [Method(WindowNames.TelemetryEvent, Direction.ServerToClient)]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))]
    public interface ITelemetryEventHandler : IJsonRpcNotificationHandler<TelemetryEventParams>
    {
    }

    public abstract class TelemetryEventHandler : ITelemetryEventHandler
    {
        public abstract Task<Unit> Handle(TelemetryEventParams request, CancellationToken cancellationToken);
    }
}
