using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class GotoArguments : IRequest<GotoResponse>
    {
        /// <summary>
        /// Set the goto target for this thread.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// The location where the debuggee will continue to run.
        /// </summary>
        public long TargetId { get; set; }
    }

}
