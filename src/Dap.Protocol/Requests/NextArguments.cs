using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Next, Direction.ClientToServer)]
    public class NextArguments : IRequest<NextResponse>
    {
        /// <summary>
        /// Execute 'next' for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// Optional granularity to step. If no granularity is specified, a granularity of 'statement' is assumed.
        /// </summary>
        [Optional]
        public SteppingGranularity Granularity { get; set; }
    }
}
