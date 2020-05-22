using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.SetFunctionBreakpoints, Direction.ClientToServer)]
    public class SetFunctionBreakpointsArguments : IRequest<SetFunctionBreakpointsResponse>
    {
        /// <summary>
        /// The function names of the breakpoints.
        /// </summary>
        public Container<FunctionBreakpoint> Breakpoints { get; set; }
    }

}
