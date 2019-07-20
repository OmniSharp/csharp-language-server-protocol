using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ReverseContinueArguments : IRequest<ReverseContinueResponse>
    {
        /// <summary>
        /// Execute 'reverseContinue' for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
