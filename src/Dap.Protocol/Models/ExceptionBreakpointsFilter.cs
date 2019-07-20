using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    /// <summary>ExceptionBreakpointsFilter
    /// An ExceptionBreakpointsFilter is shown in the UI as an option for configuring how exceptions are dealt with.
    /// </summary>
    public class ExceptionBreakpointsFilter
    {
        /// <summary>
        /// The internal ID of the filter. This value is passed to the setExceptionBreakpoints request.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// The name of the filter. This will be shown in the UI.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Initial value of the filter. If not specified a value 'false' is assumed.
        /// </summary>
        [Optional] public bool? Default { get; set; }
    }
}