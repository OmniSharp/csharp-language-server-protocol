using System;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     The base class for message handlers based on JSON-RPC <see cref="IJsonRpcHandler"/>s.
    /// </summary>
    public abstract class JsonRpcHandler
        : IHandler
    {
        /// <summary>
        ///     Create a new <see cref="JsonRpcHandler"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        protected JsonRpcHandler(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            Method = method;
        }

        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        public string Method { get; }
    }
}
