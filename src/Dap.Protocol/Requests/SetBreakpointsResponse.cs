namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class SetBreakpointsResponse
    {
        /// <summary>
        /// Information about the breakpoints.The array elements are in the same order as the elements of the 'breakpoints' (or the deprecated 'lines') array in the arguments.
        /// </summary>
        public Container<Breakpoint> breakpoints { get; set; }
    }

}
