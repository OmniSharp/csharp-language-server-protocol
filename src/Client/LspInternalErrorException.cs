using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when an internal error has occurred in the language server.
    /// </summary>
    [Serializable]
    public class LspInternalErrorException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspInternalErrorException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspInternalErrorException(string requestId)
            : base("Internal error.", requestId, LspErrorCodes.InternalError)
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
        protected LspInternalErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}