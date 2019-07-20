namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetFunctionBreakpointsResponse
    {
        /// <summary>
        /// Information about the breakpoints.The array elements correspond to the elements of the 'breakpoints' array.
        /// </summary>
        public Container<Breakpoint> breakpoints { get; set; }
    }

}
