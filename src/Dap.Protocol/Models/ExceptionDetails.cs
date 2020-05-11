using System.Text.Json.Serialization;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>ExceptionDetails
    /// Detailed information about an exception that has occurred.
    /// </summary>
    public class ExceptionDetails
    {
        /// <summary>
        /// Message contained in the exception.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string Message { get; set; }

        /// <summary>
        /// Short type name of the exception object.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string TypeName { get; set; }

        /// <summary>
        /// Fully-qualified type name of the exception object.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string FullTypeName { get; set; }

        /// <summary>
        /// Optional expression that can be evaluated in the current scope to obtain the exception object.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string EvaluateName { get; set; }

        /// <summary>
        /// Stack trace at the time the exception was thrown.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public string StackTrace { get; set; }

        /// <summary>
        /// Details of the exception contained by this exception, if any.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)] public Container<ExceptionDetails> InnerException { get; set; }
    }
}
