using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Call hierarchy options used during static registration.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensOptions : StaticTextDocumentRegistrationOptions, ISemanticTokensOptions
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
        public SemanticTokensDocumentProviderOptions DocumentProvider { get; set; }

        public bool WorkDoneProgress { get; set; }

        public static SemanticTokensOptions Of(ISemanticTokensOptions options,
            IEnumerable<IHandlerDescriptor> descriptors)
        {
            var result = new SemanticTokensOptions() {
                WorkDoneProgress = options.WorkDoneProgress,
                Legend = options.Legend ?? new SemanticTokensLegend(),
                DocumentProvider = options.DocumentProvider,
                RangeProvider = options.RangeProvider
            };
            if (result.DocumentProvider!= null && result.DocumentProvider.Edits != true)
            {
                var edits = descriptors.Any(z => z.HandlerType == typeof(ISemanticTokensEditsHandler));
                if (edits)
                {
                    result.DocumentProvider = new Supports<SemanticTokensDocumentProviderOptions>(true,
                        new SemanticTokensDocumentProviderOptions() {
                            Edits = true
                        });
                }
            }

            return result;
        }
    }
}
