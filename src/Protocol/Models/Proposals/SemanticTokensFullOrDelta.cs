using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
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

        public static implicit operator SemanticTokensFullOrDelta(SemanticTokensDelta semanticTokensDelta)
        {
            return new SemanticTokensFullOrDelta(semanticTokensDelta);
        }

        public static implicit operator SemanticTokensFullOrDelta(SemanticTokens semanticTokens)
        {
            return new SemanticTokensFullOrDelta(semanticTokens);
        }
    }
}
