using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WorkspaceClientCapabilites
    {
        /// <summary>
        /// The client supports applying batch edits
        /// to the workspace.
        /// </summary>
        public bool ApplyEdit { get; set; }

        public Supports<WorkspaceEditCapability> WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeConfiguration` notification.
        /// </summary>
        public Supports<DidChangeConfigurationCapability> DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeWatchedFiles` notification.
        /// </summary>
        public Supports<DidChangeWatchedFilesCapability> DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/symbol` request.
        /// </summary>
        public Supports<WorkspaceSymbolCapability> Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/executeCommand` request.
        /// </summary>
        public Supports<ExecuteCommandCapability> ExecuteCommand { get; set; }
    }
}
