using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensRegistrationOptions : StaticWorkDoneTextDocumentRegistrationOptions,
        ISemanticTokensOptions
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
