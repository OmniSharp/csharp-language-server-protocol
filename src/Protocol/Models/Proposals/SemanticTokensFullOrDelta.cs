using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(SemanticTokensFullOrDeltaConverter))]
    public class SemanticTokensFullOrDelta
    {
        public SemanticTokensFullOrDelta(SemanticTokensDelta delta)
        {
            Delta = delta;
            Full = null;
        }

        public SemanticTokensFullOrDelta(SemanticTokens full)
        {
            Delta = null;
            Full = full;
        }

        public SemanticTokensFullOrDelta(SemanticTokensFullOrDeltaPartialResult partialResult)
        {
            Full = null;
            Delta = null;

            if (partialResult.IsDelta)
            {
                Delta = new SemanticTokensDelta(partialResult.Delta!) {
                    Edits = partialResult.Delta!.Edits
                };
            }

            if (partialResult.IsFull)
            {
                Full = new SemanticTokens(partialResult.Full!);
            }
        }

        public bool IsFull => Full != null;
        public SemanticTokens? Full { get; }

        public bool IsDelta => Delta != null;
        public SemanticTokensDelta? Delta { get; }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokensDelta")]
        public static SemanticTokensFullOrDelta? From(SemanticTokensDelta? semanticTokensDelta) => semanticTokensDelta switch {
            not null => new(semanticTokensDelta),
            _        => null
        };

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokensDelta")]
        public static implicit operator SemanticTokensFullOrDelta?(SemanticTokensDelta? semanticTokensDelta) => semanticTokensDelta switch {
            not null => new(semanticTokensDelta),
            _        => null
        };

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
        public static SemanticTokensFullOrDelta? From(SemanticTokens? semanticTokens) => semanticTokens switch {
            not null => new(semanticTokens),
            _        => null
        };

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
        public static implicit operator SemanticTokensFullOrDelta?(SemanticTokens? semanticTokens) => semanticTokens switch {
            not null => new(semanticTokens),
            _        => null
        };

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
        public static SemanticTokensFullOrDelta? From(SemanticTokensFullOrDeltaPartialResult? semanticTokens) => semanticTokens switch {
            not null => new(semanticTokens),
            _        => null
        };

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
        public static implicit operator SemanticTokensFullOrDelta?(SemanticTokensFullOrDeltaPartialResult? semanticTokens) =>
            semanticTokens switch {
                not null => new(semanticTokens),
                _        => null
            };
    }
}
