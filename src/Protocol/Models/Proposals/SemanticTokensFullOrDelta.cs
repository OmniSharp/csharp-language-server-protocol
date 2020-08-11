using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    [JsonConverter(typeof(SemanticTokensFullOrDeltaConverter))]
    public struct SemanticTokensFullOrDelta
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

        public bool IsFull => Full != null;
        public SemanticTokens Full { get; }

        public bool IsDelta => Delta != null;
        public SemanticTokensDelta Delta { get; }

        public static implicit operator SemanticTokensFullOrDelta(SemanticTokensDelta semanticTokensDelta) => new SemanticTokensFullOrDelta(semanticTokensDelta);

        public static implicit operator SemanticTokensFullOrDelta(SemanticTokens semanticTokens) => new SemanticTokensFullOrDelta(semanticTokens);
    }
}
