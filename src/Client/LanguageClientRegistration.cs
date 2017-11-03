using OmniSharp.Extensions.JsonRpc;
using System;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client
{
    using Handlers;

    /// <summary>
    ///     Extension methods for <see cref="LanguageClient"/> enabling various styles of handler registration.
    /// </summary>
    public static class LanguageRegistration
    {
        /// <summary>
        ///     Register a handler for empty notifications.
        /// </summary>
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
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
        public static IDisposable HandleEmptyNotification(this LanguageClient languageClient, string method, EmptyNotificationHandler handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
                new DelegateEmptyNotificationHandler(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for empty notifications.
        /// </summary>
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the notification method to handle.
        /// </param>
        /// <param name="handler">
        ///     A JSON-RPC <see cref="INotificationHandler"/> that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleEmptyNotification(this LanguageClient languageClient, string method, INotificationHandler handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
                new JsonRpcEmptyNotificationHandler(method, handler)
            );
        }


        /// <summary>
        ///     Register a handler for notifications.
        /// </summary>
        /// <typeparam name="TNotification">
        ///     The notification message type.
        /// </typeparam>
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
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
        public static IDisposable HandleNotification<TNotification>(this LanguageClient languageClient, string method, NotificationHandler<TNotification> handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
                new DelegateNotificationHandler<TNotification>(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for notifications.
        /// </summary>
        /// <typeparam name="TNotification">
        ///     The notification message type.
        /// </typeparam>
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
        /// </param>
        /// <param name="method">
        ///     The name of the notification method to handle.
        /// </param>
        /// <param name="handler">
        ///     A JSON-RPC <see cref="INotificationHandler{TNotification}"/> that implements the handler.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public static IDisposable HandleNotification<TNotification>(this LanguageClient languageClient, string method, INotificationHandler<TNotification> handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
                new JsonRpcNotificationHandler<TNotification>(method, handler)
            );
        }

        /// <summary>
        ///     Register a handler for requests.
        /// </summary>
        /// <typeparam name="TRequest">
        ///     The request message type.
        /// </typeparam>
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
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
        public static IDisposable HandleRequest<TRequest>(this LanguageClient languageClient, string method, RequestHandler<TRequest> handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
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
        /// <param name="languageClient">
        ///     The <see cref="LanguageClient"/>.
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
        public static IDisposable HandleRequest<TRequest, TResponse>(this LanguageClient languageClient, string method, RequestHandler<TRequest, TResponse> handler)
        {
            if (languageClient == null)
                throw new ArgumentNullException(nameof(languageClient));

            if (String.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return languageClient.RegisterHandler(
                new DelegateRequestResponseHandler<TRequest, TResponse>(method, handler)
            );
        }
    }
}
