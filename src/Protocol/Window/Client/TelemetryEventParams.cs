using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Method(WindowNames.TelemetryEvent)]
    public class TelemetryEventParams : IRequest
    {
        [JsonExtensionData]
        private IDictionary<string, JsonElement> Data { get; set; }
    }
}
