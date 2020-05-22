using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    ///     Exception raised when an LSP request is cancelled.
    /// </summary>
    [Serializable]
    public class RequestCancelledException
        : RequestException
    {
        /// <summary>
        ///     Create a new <see cref="RequestCancelledException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public RequestCancelledException(object requestId)
            : base(ErrorCodes.RequestCancelled, requestId, "Request was cancelled.")
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
        protected RequestCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
