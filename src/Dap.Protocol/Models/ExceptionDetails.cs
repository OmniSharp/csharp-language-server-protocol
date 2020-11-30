using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Models
{
    /// <summary>
    /// ExceptionDetails
    /// Detailed information about an exception that has occurred.
    /// </summary>
    public record ExceptionDetails
    {
        /// <summary>
        /// Message contained in the exception.
        /// </summary>
        [Optional]
        public string? Message { get; init; }

        /// <summary>
        /// Short type name of the exception object.
        /// </summary>
        [Optional]
        public string? TypeName { get; init; }

        /// <summary>
        /// Fully-qualified type name of the exception object.
        /// </summary>
        [Optional]
        public string? FullTypeName { get; init; }

        /// <summary>
        /// Optional expression that can be evaluated in the current scope to obtain the exception object.
        /// </summary>
        [Optional]
        public string? EvaluateName { get; init; }

        /// <summary>
        /// Stack trace at the time the exception was thrown.
        /// </summary>
        [Optional]
        public string? StackTrace { get; init; }

        /// <summary>
        /// Details of the exception contained by this exception, if any.
        /// </summary>
        [Optional]
        public Container<ExceptionDetails>? InnerException { get; init; }
    }
}
