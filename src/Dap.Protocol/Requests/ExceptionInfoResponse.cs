using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ExceptionInfoResponse
    {
        /// <summary>
        /// ID of the exception that was thrown.
        /// </summary>
        public string ExceptionId { get; set; } = null!;

        /// <summary>
        /// Descriptive text for the exception provided by the debug adapter.
        /// </summary>
        [Optional]
        public string? Description { get; set; }

        /// <summary>
        /// Mode that caused the exception notification to be raised.
        /// </summary>
        public ExceptionBreakMode BreakMode { get; set; }

        /// <summary>
        /// Detailed information about the exception.
        /// </summary>
        [Optional]
        public ExceptionDetails? Details { get; set; }
    }
}
