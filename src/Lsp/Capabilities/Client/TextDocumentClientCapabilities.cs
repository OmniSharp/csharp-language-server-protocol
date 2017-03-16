using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentClientCapabilities
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<SynchronizationCapability> Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<CompletionCapability> Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Capability<DynamicCapability> Rename { get; set; }
    }
}