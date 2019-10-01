using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ExitedEvent : IRequest
    {

        /// <summary>
        /// The exit code returned from the debuggee.
        /// </summary>
        public long ExitCode { get; set; }
    }
}
