using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ExceptionInfoResponse
    {
        /// <summary>
        /// ID of the exception that was thrown.
        /// </summary>
        public string exceptionId { get; set; }

        /// <summary>
        /// Descriptive text for the exception provided by the debug adapter.
        /// </summary>
        [Optional] public string description { get; set; }

        /// <summary>
        /// Mode that caused the exception notification to be raised.
        /// </summary>
        public ExceptionBreakMode breakMode { get; set; }

        /// <summary>
        /// Detailed information about the exception.
        /// </summary>
        [Optional] public ExceptionDetails details { get; set; }
    }

}
