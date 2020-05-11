using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class TerminatedEvent : IRequest
    {

        /// <summary>
        /// A debug adapter may set 'restart' to true (or to an arbitrary object) to request that the front end restarts the session.
        /// The value is not interpreted by the client and passed unmodified as an attribute '__restart' to the 'launch' and 'attach' requests.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public JsonElement Restart { get; set; }
    }

}
