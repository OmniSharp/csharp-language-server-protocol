using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when an LSP request could not be parsed.
    /// </summary>
    [Serializable]
    public class LspParseErrorException
        : LspRequestException
    {
        /// <summary>
        ///     Create a new <see cref="LspParseErrorException"/>.
        /// </summary>
        /// <param name="requestId">
        ///     The LSP / JSON-RPC request Id (if known).
        /// </param>
        public LspParseErrorException(string requestId)
            : base("Error parsing request.", requestId, LspErrorCodes.ParseError)
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
        protected LspParseErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}