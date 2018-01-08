using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised an LSP request is invalid.
    /// </summary>
    [Serializable]
    public class LspInvalidRequestException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInvalidRequestException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInvalidRequestException(string requestId)
            : base("Invalid request.", requestId, LspErrorCodes.InvalidRequest)
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
        protected LspInvalidRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}