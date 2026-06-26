using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when request parameters are invalid according to the target method.
    /// </summary>
    [Serializable]
    public class UnknownErrorException
        : RequestException
    {
        /// <summary>
        /// Create a new <see cref="InvalidParametersException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        public UnknownErrorException(object? requestId)
            : base(ErrorCodes.UnknownErrorCode, requestId, "Unknown error.")
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
        protected UnknownErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
