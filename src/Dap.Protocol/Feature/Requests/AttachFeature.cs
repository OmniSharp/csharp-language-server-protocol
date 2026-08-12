using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Attach, Direction.ClientToServer)]
        [GenerateHandler(Name = "Attach", AllowDerivedRequests = true)]
        [GenerateHandlerMethods]
        [GenerateRequestMethods]
        public record AttachRequestArguments : IRequest<AttachResponse>
        {
            /// <summary>
            /// Optional data from the previous, restarted session.
            /// The data is sent as the 'restart' attribute of the 'terminated' event.
            /// The client should leave the data intact.
            /// </summary>
            [Optional]
            [JsonPropertyName("__restart")]
            public JsonElement? Restart { get; init; }

            [JsonExtensionData]
            public IDictionary<string, object> ExtensionData { get; init; } = new Dictionary<string, object>();
        }

        public record AttachResponse;
    }
}
