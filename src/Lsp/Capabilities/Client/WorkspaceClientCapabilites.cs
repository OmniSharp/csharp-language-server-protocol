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

        public Capability<WorkspaceEditCapability> WorkspaceEdit { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeConfiguration` notification.
        /// </summary>
        public Capability<DidChangeConfigurationCapability> DidChangeConfiguration { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/didChangeWatchedFiles` notification.
        /// </summary>
        public Capability<DidChangeWatchedFilesCapability> DidChangeWatchedFiles { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/symbol` request.
        /// </summary>
        public Capability<SymbolCapability> Symbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `workspace/executeCommand` request.
        /// </summary>
        public Capability<ExecuteCommandCapability> ExecuteCommand { get; set; }
    }
}
