using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetDataBreakpointsArguments : IRequest<SetDataBreakpointsResponse>
    {
        /// <summary>
        /// The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
        /// </summary>
        public Container<DataBreakpoint> Breakpoints { get; set; }
    }

}
