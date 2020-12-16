using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public class WorkspaceServerCapabilities : CapabilitiesBase, IWorkspaceServerCapabilities
    {
        /// <summary>
        /// The server supports workspace folder.
        ///
        /// Since 3.6.0
        /// </summary>
        [Optional]
        public DidChangeWorkspaceFolderRegistrationOptions.StaticOptions? WorkspaceFolders { get; set; }

        /// <summary>
        /// The server is interested in file notifications/requests.
        ///
        /// @since 3.16.0
        /// </summary>
        [Optional]
        public FileOperationsWorkspaceServerCapabilities? FileOperations { get; set; }
    }
}
