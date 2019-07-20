using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class NextArguments : IRequest<NextResponse>
    {
        /// <summary>
        /// Execute 'next' for this thread.
        /// </summary>
        public long threadId { get; set; }
    }

}
