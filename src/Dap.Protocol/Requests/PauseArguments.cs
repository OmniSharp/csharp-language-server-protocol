using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class PauseArguments : IRequest<PauseResponse>
    {
        /// <summary>
        /// Pause execution for this thread.
        /// </summary>
        public long ThreadId { get; set; }
    }

}
