using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WindowNames.TelemetryEvent, Direction.ServerToClient)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Window", AllowDerivedRequests = true),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWindowLanguageServer), typeof(ILanguageServer))
        ]
        public record TelemetryEventParams : IRequest<Unit>
        {
            [JsonExtensionData] public IDictionary<string, object> ExtensionData { get; init; } = new Dictionary<string, object>();
        }
    }

    namespace Window
    {
    }
}
