using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Terminated, Direction.ServerToClient)]
    public class TerminatedEvent : IRequest
    {

        /// <summary>
        /// A debug adapter may set 'restart' to true (or to an arbitrary object) to request that the front end restarts the session.
        /// The value is not interpreted by the client and passed unmodified as an attribute '__restart' to the 'launch' and 'attach' requests.
        /// </summary>
        [Optional] public JToken Restart { get; set; }
    }

}
