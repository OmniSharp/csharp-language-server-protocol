using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    [Method(EventNames.Exited, Direction.ServerToClient)]
    public class ExitedEvent : IRequest
    {

        /// <summary>
        /// The exit code returned from the debuggee.
        /// </summary>
        public long ExitCode { get; set; }
    }
}
