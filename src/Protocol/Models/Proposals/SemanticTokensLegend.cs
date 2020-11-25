using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    public class SemanticTokensLegend
    {
        private ImmutableDictionary<SemanticTokenModifier, int>? _tokenModifiersData;
        private ImmutableDictionary<SemanticTokenType, int>? _tokenTypesData;

        /// <summary>
        /// The token types a server uses.
        /// </summary>
        public Container<SemanticTokenType> TokenTypes { get; set; } = new Container<SemanticTokenType>(SemanticTokenType.Defaults);

        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        public Container<SemanticTokenModifier> TokenModifiers { get; set; } = new Container<SemanticTokenModifier>(SemanticTokenModifier.Defaults);

        public int GetTokenTypeIdentity(string tokenType)
        {
            EnsureTokenTypes();
            if (string.IsNullOrWhiteSpace(tokenType)) return 0;
            return _tokenTypesData != null && _tokenTypesData.TryGetValue(tokenType, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenTypeIdentity(SemanticTokenType? tokenType)
        {
            EnsureTokenTypes();
            if (!tokenType.HasValue) return 0;
            if (string.IsNullOrWhiteSpace(tokenType.Value)) return 0;
            return _tokenTypesData != null && _tokenTypesData.TryGetValue(tokenType.Value, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenModifiersIdentity(params string[]? tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Aggregate(
                       0,
                       (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                           ? acc + tokenModifer
                           : acc
                   );
        }

        public int GetTokenModifiersIdentity(IEnumerable<string>? tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Aggregate(
                       0,
                       (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                           ? acc + tokenModifer
                           : acc
                   );
        }

        public int GetTokenModifiersIdentity(params SemanticTokenModifier[]? tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Aggregate(
                       0,
                       (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                           ? acc + tokenModifer
                           : acc
                   );
        }

        public int GetTokenModifiersIdentity(IEnumerable<SemanticTokenModifier>? tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                  .Where(z => !string.IsNullOrWhiteSpace(z))
                  .Aggregate(
                       0,
                       (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                           ? acc + tokenModifer
                           : acc
                   );
        }

        private void EnsureTokenTypes() =>
            _tokenTypesData ??= TokenTypes
                               .Select(
                                    (value, index) => (
                                        value: new SemanticTokenType(value),
                                        index
                                    )
                                )
                               .Where(z => !string.IsNullOrWhiteSpace(z.value))
                               .ToImmutableDictionary(z => z.value, z => z.index);

        private void EnsureTokenModifiers() =>
            _tokenModifiersData ??= TokenModifiers
                                   .Select(
                                        (value, index) => (
                                            value: new SemanticTokenModifier(value),
                                            index
                                        )
                                    )
                                   .Where(z => !string.IsNullOrWhiteSpace(z.value))
                                   .ToImmutableDictionary(z => z.value, z => Convert.ToInt32(Math.Pow(2, z.index)));
    }
}
