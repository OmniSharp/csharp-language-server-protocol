using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class WorkspaceEdit
    {
        /// <summary>
        /// Holds changes to existing resources.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<Uri, IEnumerable<TextEdit>> Changes { get; set; }
        /// <summary>
        /// An array of `TextDocumentEdit`s to express changes to n different text documents
        /// where each text document edit addresses a specific version of a text document.
        /// Whether a client supports versioned document edits is expressed via
        /// `WorkspaceClientCapabilities.workspaceEdit.documentChanges`.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Container<TextDocumentEdit> DocumentChanges { get; set; }
    }
}
