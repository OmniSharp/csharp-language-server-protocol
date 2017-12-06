using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     A delegate-based handler for notifications.
    /// </summary>
    /// <typeparam name="TNotification">
    ///     The notification message type.
    /// </typeparam>
    public class DelegateNotificationHandler<TNotification>
        : DelegateHandler, IInvokeNotificationHandler
    {
        /// <summary>
        ///     Create a new <see cref="DelegateNotificationHandler{TNotification}"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The <see cref="NotificationHandler{TNotification}"/> delegate that implements the handler.
        /// </param>
        public DelegateNotificationHandler(string method, NotificationHandler<TNotification> handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The <see cref="NotificationHandler{TNotification}"/> delegate that implements the handler.
        /// </summary>
        public NotificationHandler<TNotification> Handler { get; }

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task Invoke(JObject notification)
        {
            await Task.Yield();

            Handler(
                notification != null ? notification.ToObject<TNotification>(Serializer.Instance.JsonSerializer /* Fix me: this is ugly */) : default(TNotification)
            );
        }
    }
}
