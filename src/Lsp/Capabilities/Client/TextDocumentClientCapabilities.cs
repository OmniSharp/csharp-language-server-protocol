using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Capabilities.Client
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TextDocumentClientCapabilities
    {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SynchronizationCapability Synchronization { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/completion`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public CompletionCapability Completion { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/hover`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability Hover { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/signatureHelp`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability SignatureHelp { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/references`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability References { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentHighlight`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability DocumentHighlight { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentSymbol`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability DocumentSymbol { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/formatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability Formatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rangeFormatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability RangeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/onTypeFormatting`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability OnTypeFormatting { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/definition`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability Definition { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeAction`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability CodeAction { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/codeLens`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability CodeLens { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/documentLink`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability DocumentLink { get; set; }

        /// <summary>
        /// Capabilities specific to the `textDocument/rename`
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DynamicCapability Rename { get; set; }
    }
}