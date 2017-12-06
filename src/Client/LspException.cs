using System;
using System.Runtime.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     Exception raised when a Language Server Protocol error is encountered.
    /// </summary>
    [Serializable]
    public class LspException
        : Exception
    {
        /// <summary>
        ///     Create a new <see cref="LspException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        public LspException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Create a new <see cref="LspException"/>.
        /// </summary>
        /// <param name="message">
        ///     The exception message.
        /// </param>
        /// <param name="inner">
        ///     The exception that caused this exception to be raised.
        /// </param>
        public LspException(string message, Exception inner)
            : base(message, inner)
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
        protected LspException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
