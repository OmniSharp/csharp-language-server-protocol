using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ReverseContinueArguments : IRequest<ReverseContinueResponse>
    {
        /// <summary>
        /// Execute 'reverseContinue' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
