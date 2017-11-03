using OmniSharp.Extensions.JsonRpc;
using System;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     An empty notification handler that invokes a JSON-RPC <see cref="INotificationHandler"/>.
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
        ///     The underlying JSON-RPC <see cref="INotificationHandler"/>.
        /// </param>
        public JsonRpcEmptyNotificationHandler(string method, INotificationHandler handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The underlying JSON-RPC <see cref="INotificationHandler"/>.
        /// </summary>
        public INotificationHandler Handler { get; }

        /// <summary>
        ///     The kind of handler.
        /// </summary>
        public override HandlerKind Kind => HandlerKind.EmptyNotification;

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task Invoke() => Handler.Handle();
    }
}
