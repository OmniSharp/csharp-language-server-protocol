using System;
using Microsoft.Extensions.Logging;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Logging
{
    /// <summary>
    ///     Extension methods for <see cref="ILogger"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        ///     <see cref="EventId"/> representing a generic error event.
        /// </summary>
        public static EventId GenericErrorEventId = new EventId(500);

        /// <summary>
        ///     Log an error.
        /// </summary>
        /// <param name="logger">
        ///     The <see cref="ILogger"/>.
        /// </param>
        /// <param name="exception">
        ///     The exception (if any) associated with the error.
        /// </param>
        /// <param name="message">
        ///     The log message.
        /// </param>
        /// <param name="args">
        ///     The message format arguments (if any).
        /// </param>
        public static void LogError(this ILogger logger, Exception exception, string message, params object[] args)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            logger.LogError(GenericErrorEventId, exception, message, args);
        }
    }
}
