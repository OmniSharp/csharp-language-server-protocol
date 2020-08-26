using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(SemanticTokensFullOrDeltaPartialResultConverter))]
    public struct SemanticTokensFullOrDeltaPartialResult
    {
        public SemanticTokensFullOrDeltaPartialResult(
            SemanticTokensPartialResult full
        )
        {
            Full = full;
            Delta = null;
        }

        public SemanticTokensFullOrDeltaPartialResult(
            SemanticTokensDeltaPartialResult delta
        )
        {
            Full = null;
            Delta = delta;
        }

        public bool IsDelta => Delta != null;
        public SemanticTokensDeltaPartialResult? Delta { get; }

        public bool IsFull => Full != null;
        public SemanticTokensPartialResult? Full { get; }

        public static implicit operator SemanticTokensFullOrDeltaPartialResult(SemanticTokensPartialResult semanticTokensPartialResult) =>
            new SemanticTokensFullOrDeltaPartialResult(semanticTokensPartialResult);

        public static implicit operator SemanticTokensFullOrDeltaPartialResult(SemanticTokensDeltaPartialResult semanticTokensDeltaPartialResult) =>
            new SemanticTokensFullOrDeltaPartialResult(semanticTokensDeltaPartialResult);
    }
}
