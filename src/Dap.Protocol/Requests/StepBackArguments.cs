using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.StepBack, Direction.ClientToServer)]
    public class StepBackArguments : IRequest<StepBackResponse>
    {
        /// <summary>
        /// Execute 'stepBack' for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
        /// </summary>
        [Optional]
        public SteppingGranularity Granularity { get; set; }
    }

}
