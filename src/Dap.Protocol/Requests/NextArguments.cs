using MediatR;
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
    }

}
