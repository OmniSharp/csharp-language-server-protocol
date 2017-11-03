namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     Represents a well-known kind of message handler.
    /// </summary>
    public enum HandlerKind
    {
        /// <summary>
        ///     A handler for empty notifications.
        /// </summary>
        EmptyNotification,

        /// <summary>
        ///     A handler for notifications.
        /// </summary>
        Notification,

        /// <summary>
        ///     A handler for requests.
        /// </summary>
        Request
    }
}
