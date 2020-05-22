using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Goto, Direction.ClientToServer)]
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
