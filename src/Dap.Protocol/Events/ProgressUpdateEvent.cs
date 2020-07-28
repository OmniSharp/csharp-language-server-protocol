using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.ProgressUpdate, Direction.ServerToClient)]
    public class ProgressUpdateEvent : ProgressEvent, IRequest
    {
        /// <summary>
        ///  Optional progress percentage to display (value range: 0 to 100). If omitted no percentage will be shown.
        /// </summary>
        [Optional]
        public int? Percentage { get; set; }
    }
}
