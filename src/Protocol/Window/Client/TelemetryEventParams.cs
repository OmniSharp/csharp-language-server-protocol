using System.Collections.Generic;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    [Method(WindowNames.TelemetryEvent)]
    public class TelemetryEventParams : IRequest
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> Data { get; set; }
    }
}
