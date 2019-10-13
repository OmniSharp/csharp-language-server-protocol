using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceFoldersServerCapabilities
    {
        /// <summary>
        /// The Server has support for workspace folders
        /// </summary>
        public bool Supported { get; set; }

        /// <summary>
        /// Whether the server wants to receive workspace folder
        /// change notifications.
        ///
        /// If a strings is provided the string is treated as a ID
        /// under which the notification is registed on the client
        /// side. The ID can be used to unregister for these events
        /// using the `client/unregisterCapability` request.
        /// </summary>
        public BooleanString ChangeNotifications { get; set; }
    }
}
