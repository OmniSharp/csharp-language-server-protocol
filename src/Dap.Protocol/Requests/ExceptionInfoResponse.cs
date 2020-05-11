using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    public class ExceptionInfoResponse
    {
        /// <summary>
        /// ID of the exception that was thrown.
        /// </summary>
        public string ExceptionId { get; set; }

        /// <summary>
        /// Descriptive text for the exception provided by the debug adapter.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Description { get; set; }

        /// <summary>
        /// Mode that caused the exception notification to be raised.
        /// </summary>
        public ExceptionBreakMode BreakMode { get; set; }

        /// <summary>
        /// Detailed information about the exception.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public ExceptionDetails Details { get; set; }
    }

}
