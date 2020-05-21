using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Capabilities, Direction.ServerToClient)]
    public class CapabilitiesEvent : IRequest
    {

        /// <summary>
        /// The set of updated capabilities.
        /// </summary>
        public Capabilities Capabilities { get; set; }
    }

}
