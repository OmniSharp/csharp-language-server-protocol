using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     A notification handler that invokes a JSON-RPC <see cref="IJsonRpcNotificationHandler{TNotification}"/>.
    /// </summary>
    /// <typeparam name="TNotification">
    ///     The notification message handler.
    /// </typeparam>
    public class JsonRpcNotificationHandler<TNotification> : JsonRpcHandler, IInvokeNotificationHandler
        where TNotification : IRequest
    {
        /// <summary>
        ///     Create a new <see cref="JsonRpcNotificationHandler{TNotification}"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The underlying JSON-RPC <see cref="IJsonRpcNotificationHandler{TNotification}"/>.
        /// </param>
        public JsonRpcNotificationHandler(string method, IJsonRpcNotificationHandler<TNotification> handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The underlying JSON-RPC <see cref="IJsonRpcNotificationHandler{TNotification}"/>.
        /// </summary>
        public IJsonRpcNotificationHandler<TNotification> Handler { get; }

        /// <summary>
        ///     The expected CLR type of the notification payload.
        /// </summary>
        public override Type PayloadType => typeof(TNotification);

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <param name="notification">
        ///     A <see cref="JObject"/> representing the notification parameters.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public Task Invoke(object notification) => Handler.Handle(
            (TNotification)notification,
            CancellationToken.None
        );
    }
}
