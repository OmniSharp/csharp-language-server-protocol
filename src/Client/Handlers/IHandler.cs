using System;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     Represents a client-side message handler.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        string Method { get; }

        /// <summary>
        ///     The expected CLR type of the request / notification payload (if any; <c>null</c> if the handler does not use the request body).
        /// </summary>
        Type PayloadType { get; }
    }
}
