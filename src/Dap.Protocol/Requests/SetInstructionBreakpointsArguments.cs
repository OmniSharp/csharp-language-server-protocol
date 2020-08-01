using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.SetInstructionBreakpoints, Direction.ClientToServer)]
    public class SetInstructionBreakpointsArguments : IRequest<SetInstructionBreakpointsResponse>
    {
        /// <summary>
        /// The contents of this array replaces all existing data breakpoints. An empty array clears all data breakpoints.
        /// </summary>
        public Container<DataBreakpoint> Breakpoints { get; set; }
    }
}
