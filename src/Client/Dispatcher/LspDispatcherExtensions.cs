using System;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Dispatcher
{
    using Handlers;

    /// <summary>
    ///     Extension methods for <see cref="LspDispatcher"/> enabling various styles of handler registration.
    /// </summary>
    public static class LspDispatcherExtensions
    {
        /// <summary>
        ///     Register a handler for empty notifications.
        /// </summary>
        /// <param name="clientDispatcher">
        ///     The <see cref="LspDispatcher"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the notification method to handle.
        /// </param>
        /// <param name="handler">
        ///     A <see cref="EmptyNotificationHandler"/> delegate that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleEmptyNotification(this LspDispatcher clientDispatcher, string method, EmptyNotificationHandler handler)
        {
            if (clientDispatcher == null)
                throw new ArgumentNullException(nameof(clientDispatcher));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientDispatcher.RegisterHandler(
                new DelegateEmptyNotificationHandler(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for notifications.
        /// </summary>
        /// <typeparam name="TNotification">
        ///     The notification message type.
        /// </typeparam>
        /// <param name="clientDispatcher">
        ///     The <see cref="LspDispatcher"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the notification method to handle.
        /// </param>
        /// <param name="handler">
        ///     A <see cref="NotificationHandler{TNotification}"/> delegate that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleNotification<TNotification>(this LspDispatcher clientDispatcher, string method, NotificationHandler<TNotification> handler)
        {
            if (clientDispatcher == null)
                throw new ArgumentNullException(nameof(clientDispatcher));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientDispatcher.RegisterHandler(
                new DelegateNotificationHandler<TNotification>(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for requests.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The request message type.
        /// </typeparam>
        /// <param name="clientDispatcher">
        ///     The <see cref="LspDispatcher"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the request method to handle.
        /// </param>
        /// <param name="handler">
        ///     A <see cref="RequestHandler{TRequest}"/> delegate that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleRequest<TRequest>(this LspDispatcher clientDispatcher, string method, RequestHandler<TRequest> handler)
        {
            if (clientDispatcher == null)
                throw new ArgumentNullException(nameof(clientDispatcher));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientDispatcher.RegisterHandler(
                new DelegateRequestHandler<TRequest>(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for requests.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The request message type.
        /// </typeparam>
        /// <typeparam name="TResponse">
        ///     The response message type.
        /// </typeparam>
        /// <param name="clientDispatcher">
        ///     The <see cref="LspDispatcher"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the request method to handle.
        /// </param>
        /// <param name="handler">
        ///     A <see cref="RequestHandler{TRequest}"/> delegate that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleRequest<TRequest, TResponse>(this LspDispatcher clientDispatcher, string method, RequestHandler<TRequest, TResponse> handler)
        {
            if (clientDispatcher == null)
                throw new ArgumentNullException(nameof(clientDispatcher));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientDispatcher.RegisterHandler(
                new DelegateRequestResponseHandler<TRequest, TResponse>(method, handler)
            );
        }
    }
}
