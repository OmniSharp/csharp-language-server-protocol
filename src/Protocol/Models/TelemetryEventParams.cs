using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(WindowNames.TelemetryEvent, Direction.ServerToClient)]
    public class TelemetryEventParams : IRequest
    {
        [JsonExtensionData] public IDictionary<string, JToken> Data { get; set; } = new Dictionary<string, JToken>();
    }
}
