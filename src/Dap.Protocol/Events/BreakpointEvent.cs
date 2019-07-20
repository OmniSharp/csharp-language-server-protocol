using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class BreakpointEvent : IRequest
    {

        /// <summary>
        /// The reason for the event.
        /// Values: 'changed', 'new', 'removed', etc.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The 'id' attribute is used to find the target breakpoint and the other attributes are used as the new values.
        /// </summary>
        public Breakpoint Breakpoint { get; set; }
    }

}
