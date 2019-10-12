using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class NextArguments : IRequest<NextResponse>
    {
        /// <summary>
        /// Execute 'next' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
