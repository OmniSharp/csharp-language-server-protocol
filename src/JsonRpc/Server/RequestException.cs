using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when a Language Server Protocol error is encountered while processing a request.
    /// </summary>
    [Serializable]
    public class RequestException : Exception, IRequestException
    {
        /// <summary>
        /// The request Id used when no valid request Id was supplied.
        /// </summary>
        public const string UnknownRequestId = "(unknown)";

        /// <summary>
        /// Create a new <see cref="RequestException" />.
        /// </summary>
        /// <param name="errorCode">
        /// The LSP / JSON-RPC error code.
        /// </param>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="message">
        /// The exception message.
        /// </param>
        public RequestException(int errorCode, object? requestId, string? message) : base(message)
        {
            RequestId = requestId ?? UnknownRequestId;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Create a new <see cref="RequestException" />.
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
        public RequestException(int errorCode, string? message, string? requestId, Exception inner) : base(message, inner)
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
        protected RequestException(SerializationInfo info, StreamingContext context)
        {
            RequestId = info.GetString(nameof(RequestId));
            ErrorCode = info.GetInt32(nameof(ErrorCode));
        }

        /// <summary>
        /// Get exception data for serialisation.
        /// </summary>
        /// <param name="info">
        /// The serialisation data-store.
        /// </param>
        /// <param name="context">
        /// The serialisation streaming context.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(RequestId), RequestId);
            info.AddValue(nameof(ErrorCode), ErrorCode);
        }

        /// <summary>
        /// The LSP / JSON-RPC request Id (if known).
        /// </summary>
        public object RequestId { get; }

        /// <summary>
        /// The LSP / JSON-RPC error code.
        /// </summary>
        public int ErrorCode { get; }
    }
}
