using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     An empty notification handler that invokes a JSON-RPC <see cref="IJsonRpcNotificationHandler"/>.
    /// </summary>
    public class JsonRpcEmptyNotificationHandler
        : JsonRpcHandler, IInvokeEmptyNotificationHandler
    {
        /// <summary>
        ///     Create a new <see cref="JsonRpcEmptyNotificationHandler"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The underlying JSON-RPC <see cref="IJsonRpcNotificationHandler"/>.
        /// </param>
        public JsonRpcEmptyNotificationHandler(string method, IJsonRpcNotificationHandler handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The underlying JSON-RPC <see cref="IJsonRpcNotificationHandler"/>.
        /// </summary>
        public IJsonRpcNotificationHandler Handler { get; }

        /// <summary>
        ///     The expected CLR type of the notification payload (<c>null</c>, since the handler does not use the request payload).
        /// </summary>
        public override Type PayloadType => null;

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task Invoke() => Handler.Handle(null, CancellationToken.None);
    }
}
