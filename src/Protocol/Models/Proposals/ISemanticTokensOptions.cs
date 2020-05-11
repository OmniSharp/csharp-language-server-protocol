using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Semantic Tokens options used during static registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public interface ISemanticTokensOptions : IWorkDoneProgressOptions
    {
        /// <summary>
        ///  The legend used by the server
        /// </summary>
        public SemanticTokensLegend Legend { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a sepcific range
        ///  of a document.
        /// </summary>

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool RangeProvider { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a full document.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public SemanticTokensDocumentProviderOptions DocumentProvider { get; set; }
    }
}
