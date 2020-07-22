using System;
using System.Collections.Immutable;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensPartialResult
    {
        public ImmutableArray<int> Data { get; set; }
    }
}
