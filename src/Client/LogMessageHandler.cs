using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     A handler for log messages sent from the language server to the client.
    /// </summary>
    /// <param name="message">
    ///     The log message.
    /// </param>
    /// <param name="messageType">
    ///     The log message type.
    /// </param>
    public delegate void LogMessageHandler(string message, MessageType messageType);
}