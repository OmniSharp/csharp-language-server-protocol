using System;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     The base class for delegate-based message handlers.
    /// </summary>
    public abstract class DelegateHandler
        : IHandler
    {
        /// <summary>
        ///     Create a new <see cref="DelegateHandler"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        protected DelegateHandler(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            Method = method;
        }

        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        public string Method { get; }

        /// <summary>
        ///     The expected CLR type of the request / notification payload (if any; <c>null</c> if the handler does not use the request payload).
        /// </summary>
        public abstract Type PayloadType { get; }
    }
}
