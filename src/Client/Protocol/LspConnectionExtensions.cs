using System;
using OmniSharp.Extensions.LanguageServer.Client.Handlers;

namespace OmniSharp.Extensions.LanguageServer.Client.Protocol
{
    /// <summary>
    ///     Extension methods for <see cref="LspConnection"/> enabling various styles of handler registration.
    /// </summary>
    public static class LspConnectionExtensions
    {
        /// <summary>
        ///     Register a handler for empty notifications.
        /// </summary>
        /// <param name="clientConnection">
        ///     The <see cref="LspConnection"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the notification method to handle.
        /// </param>
        /// <param name="handler">
        ///     A <see cref="NotificationHandler"/> delegate that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleEmptyNotification(this LspConnection clientConnection, string method, NotificationHandler handler)
        {
            if (clientConnection == null)
                throw new ArgumentNullException(nameof(clientConnection));

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientConnection.RegisterHandler(
                new DelegateEmptyNotificationHandler(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for notifications.
        /// </summary>
        /// <typeparam name="TNotification">
        ///     The notification message type.
        /// </typeparam>
        /// <param name="clientConnection">
        ///     The <see cref="LspConnection"/>.
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
        public static IDisposable HandleNotification<TNotification>(this LspConnection clientConnection, string method, NotificationHandler<TNotification> handler)
        {
            if (clientConnection == null)
                throw new ArgumentNullException(nameof(clientConnection));

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientConnection.RegisterHandler(
                new DelegateNotificationHandler<TNotification>(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for requests.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The request message type.
        /// </typeparam>
        /// <param name="clientConnection">
        ///     The <see cref="LspConnection"/>.
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
        public static IDisposable HandleRequest<TRequest>(this LspConnection clientConnection, string method, RequestHandler<TRequest> handler)
        {
            if (clientConnection == null)
                throw new ArgumentNullException(nameof(clientConnection));

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientConnection.RegisterHandler(
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
        /// <param name="clientConnection">
        ///     The <see cref="LspConnection"/>.
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
        public static IDisposable HandleRequest<TRequest, TResponse>(this LspConnection clientConnection, string method, RequestHandler<TRequest, TResponse> handler)
        {
            if (clientConnection == null)
                throw new ArgumentNullException(nameof(clientConnection));

            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return clientConnection.RegisterHandler(
                new DelegateRequestResponseHandler<TRequest, TResponse>(method, handler)
            );
        }
    }
}
