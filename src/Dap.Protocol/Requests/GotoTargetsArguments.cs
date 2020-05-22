using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.GotoTargets, Direction.ClientToServer)]
    public class GotoTargetsArguments : IRequest<GotoTargetsResponse>
    {
        /// <summary>
        /// The source location for which the goto targets are determined.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// The line location for which the goto targets are determined.
        /// </summary>
        public long Line { get; set; }

        /// <summary>
        /// An optional column location for which the goto targets are determined.
        /// </summary>
        [Optional] public long? Column { get; set; }
    }

}
