using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Module, Direction.ServerToClient)]
    public class ModuleEvent : IRequest
    {
        /// <summary>
        /// The reason for the event.
        /// </summary>
        public ModuleEventReason Reason { get; set; }

        /// <summary>
        /// The new, changed, or removed module. In case of 'removed' only the module id is used.
        /// </summary>
        public Module Module { get; set; } = null!;
    }
}
