using System;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
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
            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            Method = method;
        }

        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        public string Method { get; }

        /// <summary>
        ///     The kind of handler.
        /// </summary>
        public abstract HandlerKind Kind { get; }
    }
}
