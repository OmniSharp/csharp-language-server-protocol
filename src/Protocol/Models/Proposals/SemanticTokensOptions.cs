using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
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
    public class SemanticTokensOptions : WorkDoneProgressOptions, ISemanticTokensOptions, IStaticRegistrationOptions
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
        public BooleanOr<SemanticTokensCapabilityRequestRange> Range { get; set; }

        /// <summary>
        ///  Server supports providing semantic tokens for a full document.
        /// </summary>
        [Optional]
        public BooleanOr<SemanticTokensCapabilityRequestFull> Full { get; set; }

        public static SemanticTokensOptions Of(ISemanticTokensOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            var result = new SemanticTokensOptions() {
                WorkDoneProgress = options.WorkDoneProgress,
                Legend = options.Legend ?? new SemanticTokensLegend(),
                Full = options.Full,
                Range = options.Range
            };
            if (result.Full != null && result.Full?.Value.Delta != true)
            {
                var edits = descriptors.Any(z => z.HandlerType == typeof(ISemanticTokensDeltaHandler));
                if (edits)
                {
                    result.Full = new BooleanOr<SemanticTokensCapabilityRequestFull>(new SemanticTokensCapabilityRequestFull() {
                        Delta = true
                    });
                }
            }

            return result;
        }

        public string Id { get; set; }
    }
}
