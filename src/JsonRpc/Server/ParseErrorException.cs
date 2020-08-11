using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    /// Exception raised when an LSP request could not be parsed.
    /// </summary>
    [Serializable]
    public class ParseErrorException
        : RequestException
    {
        /// <summary>
        /// Create a new <see cref="ParseErrorException" />.
        /// </summary>
        /// <param name="requestId">
        /// The LSP / JSON-RPC request Id (if known).
        /// </param>
        public ParseErrorException(object requestId)
            : base(ErrorCodes.ParseError, requestId, "Error parsing request.")
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
        protected ParseErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
