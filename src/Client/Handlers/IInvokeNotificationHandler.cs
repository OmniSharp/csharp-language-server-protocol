﻿using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     Represents a handler for notifications.
    /// </summary>
    public interface IInvokeNotificationHandler
        : IHandler
    {
        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <param name="notification">
        ///     The notification message.
        /// </param>
        /// <returns>
        ///     A <see cref="Task"/> representing the operation.
        /// </returns>
        Task Invoke(object notification);
    }
}
