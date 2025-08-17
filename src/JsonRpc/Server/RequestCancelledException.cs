using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when an LSP request is cancelled.
    /// </summary>
    [Serializable]
    public class RequestCancelledException
        : TaskCanceledException, IRequestException
    {
        /// <summary>
        /// Create a new <see cref="RequestCancelledException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        public RequestCancelledException(object? requestId)
            : this(ErrorCodes.RequestCancelled, requestId?.ToString() ?? "(unknown)", "Request was cancelled.", null!)
        {
        }

        /// <summary>
        /// Create a new <see cref="RequestCancelledException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="inner">
        /// The exception that caused this exception to be raised.
        /// </param>
        public RequestCancelledException(object? requestId, Exception inner)
            : this(ErrorCodes.RequestCancelled, requestId?.ToString() ?? "(unknown)", "Request was cancelled.", inner)
        {
        }

        /// <summary>
        /// Create a new <see cref="RequestCancelledException" />.
        /// </summary>
        /// <param name="errorCode">
        /// The LSP / JSON-RPC error code.
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="inner">
        /// The exception that caused this exception to be raised.
        /// </param>
        public RequestCancelledException(int errorCode, string message, string requestId, Exception inner) : base(message, inner)
        {
            RequestId = !string.IsNullOrWhiteSpace(requestId) ? requestId : UnknownRequestId;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Serialisation constructor.
        /// </summary>
        /// <param name="info">
        /// The serialisation data-store.
        /// </param>
        /// <param name="context">
        /// The serialisation streaming context.
        /// </param>
        protected RequestCancelledException(SerializationInfo info, StreamingContext context)
        {
            RequestId = info.GetString(nameof(RequestId));
            ErrorCode = info.GetInt32(nameof(ErrorCode));
        }

        /// <summary>
        /// The LSP / JSON-RPC request Id (if known).
        /// </summary>
        public object RequestId { get; }

        /// <summary>
        /// The LSP / JSON-RPC error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// The request Id used when no valid request Id was supplied.
        /// </summary>
        public const string UnknownRequestId = "(unknown)";
    }
}
