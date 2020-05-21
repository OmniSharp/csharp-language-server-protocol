using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    ///     Exception raised an LSP request is invalid.
    /// </summary>
    [Serializable]
    public class InvalidRequestException
        : RequestException
    {
        /// <summary>
        ///     Create a new <see cref="InvalidRequestException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public InvalidRequestException(object requestId)
            : base(ErrorCodes.InvalidRequest, requestId, "Invalid request.")
        {
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
        protected InvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
