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
        [Method(RequestNames.Attach, Direction.ClientToServer)]
        [
            GenerateHandler(Name = "Attach"),
            GenerateHandlerMethods(AllowDerivedRequests = true),
            GenerateRequestMethods
        ]
        public class AttachRequestArguments : IRequest<AttachResponse>
        {
            /// <summary>
            /// Optional data from the previous, restarted session.
            /// The data is sent as the 'restart' attribute of the 'terminated' event.
            /// The client should leave the data intact.
            /// </summary>
            [Optional]
            [JsonProperty(PropertyName = "__restart")]
            public JToken? Restart { get; set; }

            [JsonExtensionData] public IDictionary<string, object> ExtensionData { get; set; } = new Dictionary<string, object>();
        }

        public class AttachResponse
        {
        }
    }
}
