using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetFunctionBreakpointsArguments : IRequest<SetFunctionBreakpointsResponse>
    {
        /// <summary>
        /// The function names of the breakpoints.
        /// </summary>
        public Container<FunctionBreakpoint> Breakpoints { get; set; }
    }

}
