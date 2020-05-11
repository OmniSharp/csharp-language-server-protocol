using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    ///  Server supports providing semantic tokens for a full document.
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensDocumentProviderOptions
    {
        /// <summary>
        ///  The server supports deltas for full documents.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool? Edits { get; set; }
    }
}
