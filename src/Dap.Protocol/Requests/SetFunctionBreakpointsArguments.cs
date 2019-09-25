using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class SetFunctionBreakpointsArguments : IRequest<SetFunctionBreakpointsResponse>
    {
        /// <summary>
        /// The function names of the breakpoints.
        /// </summary>
        public Container<FunctionBreakpoint> Breakpoints { get; set; }
    }

}
