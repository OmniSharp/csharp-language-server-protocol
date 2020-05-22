using System;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public struct SemanticTokensPartialResultOrSemanticTokensEditsPartialResult
    {
        public SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensEditsPartialResult semanticTokensEditsPartialResult)
        {
            SemanticTokensEditsPartialResult = semanticTokensEditsPartialResult;
            SemanticTokensPartialResult = null;
        }

        public SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensPartialResult semanticTokensPartialResult)
        {
            SemanticTokensEditsPartialResult = null;
            SemanticTokensPartialResult = semanticTokensPartialResult;
        }

        public bool IsSemanticTokensPartialResult => SemanticTokensPartialResult != null;
        public SemanticTokensPartialResult SemanticTokensPartialResult { get; }

        public bool IsSemanticTokensEditsPartialResult => SemanticTokensEditsPartialResult != null;
        public SemanticTokensEditsPartialResult SemanticTokensEditsPartialResult { get; }

        public static implicit operator SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensEditsPartialResult semanticTokensEditsPartialResult)
        {
            return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(semanticTokensEditsPartialResult);
        }

        public static implicit operator SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(
            SemanticTokensPartialResult semanticTokensPartialResult)
        {
            return new SemanticTokensPartialResultOrSemanticTokensEditsPartialResult(semanticTokensPartialResult);
        }
    }
}
