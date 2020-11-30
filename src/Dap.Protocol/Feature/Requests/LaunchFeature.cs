using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    namespace Requests
    {
        [Parallel]
        [Method(RequestNames.Launch, Direction.ClientToServer)]
        [
            GenerateHandler(Name = "Launch", AllowDerivedRequests = true),
            GenerateHandlerMethods,
            GenerateRequestMethods
        ]
        public record LaunchRequestArguments : IRequest<LaunchResponse>
        {
            /// <summary>
            /// If noDebug is true the launch request should launch the program without enabling debugging.
            /// </summary>
            [Optional]
            public bool NoDebug { get; init; }

            /// <summary>
            /// Optional data from the previous, restarted session.
            /// The data is sent as the 'restart' attribute of the 'terminated' event.
            /// The client should leave the data intact.
            /// </summary>
            [Optional]
            [JsonProperty(PropertyName = "__restart")]
            public JToken? Restart { get; init; }

            [JsonExtensionData] public IDictionary<string, object> ExtensionData { get; init; } = new Dictionary<string, object>();
        }

        public record LaunchResponse
        {
        }
    }
}
