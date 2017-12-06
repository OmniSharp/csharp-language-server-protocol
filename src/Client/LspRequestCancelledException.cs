using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when an LSP request is cancelled.
    /// </summary>
    [Serializable]
    public class LspRequestCancelledException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspRequestCancelledException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspRequestCancelledException(string requestId)
            : base("Request was cancelled.", requestId, LspErrorCodes.RequestCancelled)
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
        protected LspRequestCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}