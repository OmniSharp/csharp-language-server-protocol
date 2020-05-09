using System;
using System.Threading.Tasks;

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
        ///     The expected CLR type of the notification payload.
        /// </summary>
        public override Type PayloadType => typeof(TNotification);

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        public async Task Invoke(object notification)
        {
            await Task.Yield();

            Handler(
                (TNotification)notification
            );
        }
    }
}
