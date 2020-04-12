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
        private ImmutableDictionary<string, int> _stringTokenModifiers;
        private ImmutableDictionary<SemanticTokenModifiers?, int> _enumTokenModifiers;
        private ImmutableDictionary<string, int> _stringTokenTypes;
        private ImmutableDictionary<SemanticTokenTypes?, int> _enumTokenTypes;
        private Container<string> _tokenTypes;
        private Container<string> _tokenModifiers;

        /// <summary>
        /// The token types a server uses.
        /// </summary>
        public Container<string> TokenTypes
        {
            get => _tokenTypes;
            set {
                _tokenTypes = value;
                _enumTokenTypes = null;
                _stringTokenTypes = null;
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
                _enumTokenModifiers = null;
                _stringTokenModifiers = null;
            }
        }

        public int GetTokenTypeIdentity(string tokenType)
        {
            EnsureTokenTypes();
            if (string.IsNullOrWhiteSpace(tokenType)) return 0;
            return _stringTokenTypes.TryGetValue(tokenType, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenTypeIdentity(SemanticTokenTypes? tokenType)
        {
            EnsureTokenTypes();
            if (!tokenType.HasValue) return 0;
            return _enumTokenTypes.TryGetValue(tokenType, out var tokenTypeNumber) ? tokenTypeNumber : 0;
        }

        public int GetTokenModifiersIdentity(params string[] tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers.Aggregate(0,
                (acc, value) => _stringTokenModifiers.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(IEnumerable<string> tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers.Aggregate(0,
                (acc, value) => _stringTokenModifiers.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(params SemanticTokenModifiers[] tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers.Aggregate(0,
                (acc, value) => _enumTokenModifiers.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        public int GetTokenModifiersIdentity(IEnumerable<SemanticTokenModifiers> tokenModifiers)
        {
            EnsureTokenModifiers();
            if (tokenModifiers == null) return 0;
            return tokenModifiers.Aggregate(0,
                (acc, value) => _enumTokenModifiers.TryGetValue(value, out var tokenModifer)
                    ? acc + tokenModifer
                    : acc);
        }

        private void EnsureTokenTypes()
        {
            _stringTokenTypes ??= TokenTypes
                .Select((value, index) => (value, index))
                .ToImmutableDictionary(z => z.value, z => z.index, StringComparer.OrdinalIgnoreCase);
            _enumTokenTypes ??= TokenTypes
                .Select((value, index) => (
                    value: Enum.TryParse<SemanticTokenTypes>(value, out var result)
                        ? new SemanticTokenTypes?(result)
                        : null, index))
                .ToImmutableDictionary(z => z.value, z => z.index);
        }

        private void EnsureTokenModifiers()
        {
            _stringTokenModifiers ??= TokenModifiers
                .Select((value, index) => (value, index))
                .ToImmutableDictionary(z => z.value, z => Convert.ToInt32(Math.Pow(2, z.index)),
                    StringComparer.OrdinalIgnoreCase);
            _enumTokenModifiers ??= TokenModifiers
                .Select((value, index) => (
                    value: Enum.TryParse<SemanticTokenModifiers>(value, out var result)
                        ? new SemanticTokenModifiers?(result)
                        : null, index))
                .ToImmutableDictionary(z => z.value, z => Convert.ToInt32(Math.Pow(2, z.index)));
        }
    }
}