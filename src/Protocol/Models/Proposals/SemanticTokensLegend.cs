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
        private ImmutableDictionary<SemanticTokenModifier, int> _tokenModifiersData;
        private ImmutableDictionary<SemanticTokenType, int> _tokenTypesData;

        private Container<string> _tokenTypes = new Container<string>(SemanticTokenType
            .Defaults
                .Select(z => (string) z)
                .ToArray()
        );

        private Container<string> _tokenModifiers = new Container<string>(
            SemanticTokenModifier
                .Defaults
                .Select(z => (string) z)
                .ToArray());

        /// <summary>
        /// The token types a server uses.
        /// </summary>
        public Container<string> TokenTypes
        {
            get => _tokenTypes;
            set {
                _tokenTypes = value;
                _tokenTypesData = null;
            }
        }

        /// <summary>
        /// The token modifiers a server uses.
        /// </summary>
        public Container<string> TokenModifiers
        {
            get => _tokenModifiers;
            set {
                _tokenModifiers = value;
                _tokenModifiersData = null;
            }
        }

        public int GetTokenTypeIdentity(string tokenType)
        {
            EnsureTokenTypes();
            if (string.IsNullOrWhiteSpace(tokenType)) return 0;
            return _tokenTypesData.TryGetValue(tokenType, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenTypeIdentity(SemanticTokenType? tokenType)
        {
            EnsureTokenTypes();
            if (!tokenType.HasValue) return 0;
            if (string.IsNullOrWhiteSpace((tokenType.Value))) return 0;
            return _tokenTypesData.TryGetValue(tokenType.Value, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenModifiersIdentity(params string[] tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .Aggregate(0,
                (acc, value) => _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(IEnumerable<string> tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .Aggregate(0,
                (acc, value) => _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(params SemanticTokenModifier[] tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .Aggregate(0,
                (acc, value) => _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(IEnumerable<SemanticTokenModifier> tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers
                .Where(z => !string.IsNullOrWhiteSpace(z))
                .Aggregate(0,
                (acc, value) => _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        private void EnsureTokenTypes()
        {
            _tokenTypesData ??= TokenTypes
                .Select((value, index) => (
                    value: new SemanticTokenType(value),
                    index
                ))
                .Where(z => !string.IsNullOrWhiteSpace(z.value))
                .ToImmutableDictionary(z => z.value, z => z.index);
        }

        private void EnsureTokenModifiers()
        {
            _tokenModifiersData ??= TokenModifiers
                .Select((value, index) => (
                    value: new SemanticTokenModifier(value),
                    index
                ))
                .Where(z => !string.IsNullOrWhiteSpace(z.value))
                .ToImmutableDictionary(z => z.value, z => Convert.ToInt32(Math.Pow(2, z.index)));
        }
    }
}
