using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Server
{
    /// <summary>
    ///     Exception raised when request parameters are invalid according to the target method.
    /// </summary>
    [Serializable]
    public class InvalidParametersException
        : RequestException
    {
        /// <summary>
        ///     Create a new <see cref="InvalidParametersException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public InvalidParametersException(object requestId)
            : base(ErrorCodes.InvalidParameters, requestId, "Invalid parameters.")
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
        protected InvalidParametersException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
