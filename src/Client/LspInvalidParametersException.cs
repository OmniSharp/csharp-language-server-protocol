using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when request parameters are invalid according to the target method.
    /// </summary>
    [Serializable]
    public class LspInvalidParametersException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInvalidParametersException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInvalidParametersException(string requestId)
            : base("Invalid parameters.", requestId, LspErrorCodes.InvalidParameters)
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
        protected LspInvalidParametersException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}