using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public struct SemanticTokensOrSemanticTokensEdits
    {
        public SemanticTokensOrSemanticTokensEdits(SemanticTokensEdits semanticTokensEdits)
        {
            SemanticTokensEdits = semanticTokensEdits;
            SemanticTokens = null;
        }

        public SemanticTokensOrSemanticTokensEdits(SemanticTokens semanticTokens)
        {
            SemanticTokensEdits = null;
            SemanticTokens = semanticTokens;
        }

        public bool IsSemanticTokens => SemanticTokens != null;
        public SemanticTokens SemanticTokens { get; }

        public bool IsSemanticTokensEdits => SemanticTokensEdits != null;
        public SemanticTokensEdits SemanticTokensEdits { get; }

        public static implicit operator SemanticTokensOrSemanticTokensEdits(SemanticTokensEdits semanticTokensEdits)
        {
            return new SemanticTokensOrSemanticTokensEdits(semanticTokensEdits);
        }

        public static implicit operator SemanticTokensOrSemanticTokensEdits(SemanticTokens semanticTokens)
        {
            return new SemanticTokensOrSemanticTokensEdits(semanticTokens);
        }
    }
}
