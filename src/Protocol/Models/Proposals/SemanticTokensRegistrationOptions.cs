using System;
using System.Collections.Generic;
using System.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensRegistrationOptions : StaticWorkDoneTextDocumentRegistrationOptions
    {
        /// <summary>
        /// The legend used by the server
        /// </summary>
        public SemanticTokensLegend Legend { get; set; }

        /// <summary>
        /// Server supports providing semantic tokens for a specific range
        /// of a document.
        /// </summary>
        [Optional]
        public BooleanOr<SemanticTokensCapabilityRequestRange> Range { get; set; }

        /// <summary>
        /// Server supports providing semantic tokens for a full document.
        /// </summary>
        [Optional]
        public BooleanOr<SemanticTokensCapabilityRequestFull> Full { get; set; }

        /// <summary>
        /// Call hierarchy options used during static registration.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public class StaticOptions : WorkDoneProgressOptions, IStaticRegistrationOptions
        {
            /// <summary>
            /// The legend used by the server
            /// </summary>
            public SemanticTokensLegend Legend { get; set; }

            /// <summary>
            /// Server supports providing semantic tokens for a specific range
            /// of a document.
            /// </summary>
            [Optional]
            public BooleanOr<SemanticTokensCapabilityRequestRange> Range { get; set; }

            /// <summary>
            /// Server supports providing semantic tokens for a full document.
            /// </summary>
            [Optional]
            public BooleanOr<SemanticTokensCapabilityRequestFull> Full { get; set; }

            public string Id { get; set; }
        }

        class SemanticTokensRegistrationOptionsConverter : RegistrationOptionsConverterBase<SemanticTokensRegistrationOptions, StaticOptions>
        {
            private readonly IHandlersManager _handlersManager;

            public SemanticTokensRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.SemanticTokensProvider))
            {
                _handlersManager = handlersManager;
            }
            public override StaticOptions Convert(SemanticTokensRegistrationOptions source)
            {
                var result = new StaticOptions {
                    WorkDoneProgress = source.WorkDoneProgress,
                    Legend = source.Legend ?? new SemanticTokensLegend(),
                    Full = source.Full,
                    Range = source.Range
                };
                if (result.Full != null && result.Full?.Value.Delta != true)
                {
                    var edits = _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ISemanticTokensDeltaHandler));
                    if (edits)
                    {
                        result.Full = new BooleanOr<SemanticTokensCapabilityRequestFull>(
                            new SemanticTokensCapabilityRequestFull {
                                Delta = true
                            }
                        );
                    }
                }

                return result;
            }
        }
    }
}
