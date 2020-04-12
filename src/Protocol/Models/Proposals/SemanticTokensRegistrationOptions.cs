using System;
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

        [Optional]
        public bool RangeProvider { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a full document.
        /// </summary>
        [Optional]
        public Supports<SemanticTokensDocumentProviderOptions> DocumentProvider { get; set; }
    }
}