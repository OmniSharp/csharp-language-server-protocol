using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    public interface IWorkspaceServerCapabilities : ICapabilitiesBase
    {
        /// <summary>
        /// The server supports workspace folder.
        ///
        /// Since 3.6.0
        /// </summary>
        DidChangeWorkspaceFolderRegistrationOptions.StaticOptions? WorkspaceFolders { get; set; }

        /// <summary>
        /// The server is interested in file notifications/requests.
        ///
        /// @since 3.16.0
        /// </summary>
        FileOperationsWorkspaceServerCapabilities? FileOperations { get; set; }
    }
}
