using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class GotoArguments : IRequest<GotoResponse>
    {
        /// <summary>
        /// Set the goto target for this thread.
        /// </summary>
        public long threadId { get; set; }

        /// <summary>
        /// The location where the debuggee will continue to run.
        /// </summary>
        public long targetId { get; set; }
    }

}
