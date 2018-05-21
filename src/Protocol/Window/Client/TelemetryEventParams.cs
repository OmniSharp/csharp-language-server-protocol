using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable CheckNamespace

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class TelemetryEventParams : IRequest
    {
        [JsonExtensionData]
        private IDictionary<string, JToken> Data { get; set; }
    }
}
