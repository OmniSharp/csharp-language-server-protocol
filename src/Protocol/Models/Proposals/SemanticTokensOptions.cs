using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Call hierarchy options used during static registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensOptions : WorkDoneProgressOptions, ISemanticTokensOptions
    {
        /// <summary>
        ///  The legend used by the server
        /// </summary>
        public SemanticTokensLegend Legend { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a specific range
        ///  of a document.
        /// </summary>
        [Optional]
        public bool RangeProvider { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a full document.
        /// </summary>
        [Optional]
        public Supports<SemanticTokensDocumentProviderOptions> DocumentProvider { get; set; }

        public static SemanticTokensOptions Of(ISemanticTokensOptions options)
        {
            return new SemanticTokensOptions() {
                WorkDoneProgress = options.WorkDoneProgress,
                Legend = options.Legend,
                DocumentProvider = options.DocumentProvider,
                RangeProvider = options.RangeProvider
            };
        }
    }
}