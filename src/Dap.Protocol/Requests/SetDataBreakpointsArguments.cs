using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetDataBreakpointsArguments : IRequest<SetDataBreakpointsResponse>
    {
        /// <summary>
        /// The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
        /// </summary>
        public Container<DataBreakpoint> breakpoints { get; set; }
    }

}
