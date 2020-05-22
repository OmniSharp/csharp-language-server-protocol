using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.LoadedSource, Direction.ServerToClient)]
    public class LoadedSourceEvent : IRequest
    {
        /// <summary>
        /// The reason for the event.
        /// </summary>
        public LoadedSourceReason Reason { get; set; }

        /// <summary>
        /// The new, changed, or removed source.
        /// </summary>
        public Source Source { get; set; }
    }

}
