using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ModuleEvent : IRequest
    {

        /// <summary>
        /// The reason for the event.
        /// </summary>
        public ModuleEventReason Reason { get; set; }

        /// <summary>
        /// The new, changed, or removed module. In case of 'removed' only the module id is used.
        /// </summary>
        public Module Module { get; set; }
    }

}
