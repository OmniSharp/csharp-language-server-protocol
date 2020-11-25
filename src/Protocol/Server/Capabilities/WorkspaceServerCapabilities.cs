using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class WorkspaceServerCapabilities : CapabilitiesBase
    {
        /// <summary>
        /// The server supports workspace folder.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public WorkspaceFolderOptions? WorkspaceFolders { get; set; }
    }
}
