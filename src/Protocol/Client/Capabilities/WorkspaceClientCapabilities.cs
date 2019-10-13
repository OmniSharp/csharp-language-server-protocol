using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class WorkspaceClientCapabilities
    {
        /// <summary>
        /// The client supports applying batch edits
        /// to the workspace.
        /// </summary>
        public Supports<bool> ApplyEdit { get; set; }

        public Supports<WorkspaceEditClientCapabilities> WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeConfiguration` notification.
        /// </summary>
        public Supports<DidChangeConfigurationClientCapabilities> DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeWatchedFiles` notification.
        /// </summary>
        public Supports<DidChangeWatchedFilesClientCapabilities> DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/symbol` request.
        /// </summary>
        public Supports<WorkspaceSymbolClientCapabilities> Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/executeCommand` request.
        /// </summary>
        public Supports<ExecuteCommandClientCapabilities> ExecuteCommand { get; set; }

        /// <summary>
        /// The client has support for workspace folders.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<bool> WorkspaceFolders { get; set; }

        /// <summary>
        /// The client supports `workspace/configuration` requests.
        ///
        /// Since 3.6.0
        /// </summary>
        public Supports<bool> Configuration { get; set; }
    }
}
