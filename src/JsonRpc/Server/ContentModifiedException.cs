using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    ///     Exception raised when request parameters are invalid according to the target method.
    /// </summary>
    [Serializable]
    public class ContentModifiedException
        : TaskCanceledException, IRequestException
    {
        /// <summary>
        ///     Create a new <see cref="ContentModifiedException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public ContentModifiedException(object requestId)
            : this(ErrorCodes.ContentModified, requestId.ToString(), "Content not modified.", null)
        {
        }

        /// <summary>
        ///     Create a new <see cref="ContentModifiedException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public ContentModifiedException(object requestId, Exception inner)
            : this(ErrorCodes.ContentModified, requestId.ToString(), "Content not modified.", inner)
        {
        }

        /// <summary>
        ///     Create a new <see cref="ContentModifiedException"/>.
        /// </summary>
        /// <param name="errorCode">
        ///     The LSP / JSON-RPC error code.
        /// </param>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public ContentModifiedException(int errorCode, string message, string requestId, Exception inner) : base(message, inner)
        {
            RequestId = !string.IsNullOrWhiteSpace(requestId) ? requestId : UnknownRequestId;
            ErrorCode = errorCode;
        }

        /// <summary>
        ///     Serialisation constructor.
        /// </summary>
        /// <param name="info">
        ///     The serialisation data-store.
        /// </param>
        /// <param name="context">
        ///     The serialisation streaming context.
        /// </param>
        protected ContentModifiedException(SerializationInfo info, StreamingContext context)
        {
            RequestId = info.GetString(nameof(RequestId));
            ErrorCode = info.GetInt32(nameof(ErrorCode));
        }

        /// <summary>
        ///     The LSP / JSON-RPC request Id (if known).
        /// </summary>
        public object RequestId { get; }

        /// <summary>
        ///     The LSP / JSON-RPC error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        ///     The request Id used when no valid request Id was supplied.
        /// </summary>
        public const string UnknownRequestId = "(unknown)";
    }
}
