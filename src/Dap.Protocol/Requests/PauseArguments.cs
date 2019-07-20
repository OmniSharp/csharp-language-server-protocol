using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class PauseArguments : IRequest<PauseResponse>
    {
        /// <summary>
        /// Pause execution for this thread.
        /// </summary>
        public long threadId { get; set; }
    }

}
