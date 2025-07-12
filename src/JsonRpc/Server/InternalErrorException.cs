using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when an internal error has occurred in the language server.
    /// </summary>
    [Serializable]
    public class InternalErrorException
        : RequestException
    {
        /// <summary>
        /// Create a new <see cref="InternalErrorException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        /// <param name="message"></param>
        public InternalErrorException(object? requestId, string? message)
            : base(ErrorCodes.InternalError, requestId, "Internal error. " + message)
        {
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
        protected InternalErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
