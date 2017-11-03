using System;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     A delegate-based handler for empty notifications.
    /// </summary>
    public class DelegateEmptyNotificationHandler
        : DelegateHandler, IInvokeEmptyNotificationHandler
    {
        /// <summary>
        ///     Create a new <see cref="DelegateEmptyNotificationHandler"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The <see cref="EmptyNotificationHandler"/> delegate that implements the handler.
        /// </param>
        public DelegateEmptyNotificationHandler(string method, EmptyNotificationHandler handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The <see cref="EmptyNotificationHandler"/> delegate that implements the handler.
        /// </summary>
        public EmptyNotificationHandler Handler { get; }

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
        public async Task Invoke()
        {
            await Task.Yield();

            Handler();
        }
    }
}
