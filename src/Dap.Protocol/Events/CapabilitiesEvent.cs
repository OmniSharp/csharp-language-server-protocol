using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class CapabilitiesEvent : IRequest
    {

        /// <summary>
        /// The set of updated capabilities.
        /// </summary>
        public Capabilities Capabilities { get; set; }
    }

}
