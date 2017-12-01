using System;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client.Clients
{
    /// <summary>
    ///     Client for the LSP Window API.
    /// </summary>
    public class WindowClient
    {
        /// <summary>
        ///     Create a new <see cref="WindowClient"/>.
        /// </summary>
        /// <param name="client">
        ///     The language client providing the API.
        /// </param>
        public WindowClient(LanguageClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Client = client;
        }

        /// <summary>
        ///     The language client providing the API.
        /// </summary>
        public LanguageClient Client { get; }

        /// <summary>
        ///     Register a handler for "window/logMessage" notifications from the server.
        /// </summary>
        /// <param name="handler">
        ///     The <see cref="LogMessageHandler"/> that will be called for each log message.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the registration.
        /// </returns>
        public IDisposable OnLogMessage(LogMessageHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            return Client.HandleNotification<LogMessageParams>(WindowNames.LogMessage,
                notification => handler(notification.Message, notification.Type)
            );
        }
    }
}
