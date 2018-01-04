using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Client.Handlers;

namespace OmniSharp.Extensions.LanguageServer.Client.Dispatcher
{
    /// <summary>
    ///     Dispatches requests and notifications from a language server to a language client.
    /// </summary>
    public class LspDispatcher
    {
        /// <summary>
        ///     Invokers for registered handlers.
        /// </summary>
        readonly ConcurrentDictionary<string, IHandler> _handlers = new ConcurrentDictionary<string, IHandler>();

        /// <summary>
        ///     Create a new <see cref="LspDispatcher"/>.
        /// </summary>
        /// <param name="serializer">
        ///     The JSON serialiser for notification / request / response payloads.
        /// </param>
        public LspDispatcher(ISerializer serializer)
        {
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            Serializer = serializer;
        }

        /// <summary>
        ///     The JSON serialiser to use for notification / request / response payloads.
        /// </summary>
        public ISerializer Serializer { get; set; }

        /// <summary>
        ///     Register a handler invoker.
        /// </summary>
        /// <param name="handler">
        ///     The handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public IDisposable RegisterHandler(IHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            string method = handler.Method;

            if (!_handlers.TryAdd(method, handler))
                throw new InvalidOperationException($"There is already a handler registered for method '{handler.Method}'.");

            return Disposable.Create(
                () => _handlers.TryRemove(method, out _)
            );
        }

        /// <summary>
        ///     Attempt to handle an empty notification.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if an empty notification handler was registered for specified method; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> TryHandleEmptyNotification(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (_handlers.TryGetValue(method, out IHandler handler) && handler is IInvokeEmptyNotificationHandler emptyNotificationHandler)
            {
                await emptyNotificationHandler.Invoke();

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempt to handle a notification.
        /// </summary>
        /// <param name="method">
        ///     The notification method name.
        /// </param>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if a notification handler was registered for specified method; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> TryHandleNotification(string method, JObject notification)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (_handlers.TryGetValue(method, out IHandler handler) && handler is IInvokeNotificationHandler notificationHandler)
            {
                object notificationPayload = DeserializePayload(notificationHandler.PayloadType, notification);

                await notificationHandler.Invoke(notificationPayload);

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Attempt to handle a request.
        /// </summary>
        /// <param name="method">
        ///     The request method name.
        /// </param>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     If a registered handler was found, a <see cref="Task"/> representing the operation; otherwise, <c>null</c>.
        /// </returns>
        public Task<object> TryHandleRequest(string method, JObject request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (_handlers.TryGetValue(method, out IHandler handler) && handler is IInvokeRequestHandler requestHandler)
            {
                object requestPayload = DeserializePayload(requestHandler.PayloadType, request);

                return requestHandler.Invoke(requestPayload, cancellationToken);
            }

            return null;
        }

        /// <summary>
        ///     Deserialise a notification / request payload from JSON.
        /// </summary>
        /// <param name="payloadType">
        ///     The payload's CLR type.
        /// </param>
        /// <param name="payload">
        ///     JSON representing the payload.
        /// </param>
        /// <returns>
        ///     The deserialised payload (if one is present and expected).
        /// </returns>
        object DeserializePayload(Type payloadType, JObject payload)
        {
            if (payloadType == null)
                throw new ArgumentNullException(nameof(payloadType));

            if (payloadType == null || payload == null)
                return null;

            return payload.ToObject(payloadType, Serializer.JsonSerializer);
        }
    }
}
