using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// The protocol defines an additional token format capability to allow future extensions of the format.
    /// The only format that is currently specified is `relative` expressing that the tokens are described using relative positions.
    ///
    /// @since 3.16.0
    /// </summary>
    [JsonConverter(typeof(EnumLikeStringConverter))]
    [Obsolete(Constants.Proposal)]
    [DebuggerDisplay("{_value}")]
    public readonly struct SemanticTokenFormat : IEquatable<SemanticTokenFormat>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<SemanticTokenFormat>> _defaults =
            new Lazy<IReadOnlyList<SemanticTokenFormat>>(
                () => {
                    return typeof(SemanticTokenFormat)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(z => z.GetValue(null))
                        .Cast<SemanticTokenFormat>()
                        .ToArray();
                });

        public static IEnumerable<SemanticTokenFormat> Defaults => _defaults.Value;

        public static readonly SemanticTokenFormat Relative = new SemanticTokenFormat("relative");

        private readonly string _value;

        public SemanticTokenFormat(string modifier)
        {
            _value = modifier;
        }

        public static implicit operator SemanticTokenFormat(string kind)
        {
            return new SemanticTokenFormat(kind);
        }

        public static implicit operator string(SemanticTokenFormat kind)
        {
            return kind._value;
        }

        public override string ToString() => _value;
        public bool Equals(SemanticTokenFormat other) => _value == other._value;

        public override bool Equals(object obj) => obj is SemanticTokenFormat other && Equals(other);

        public override int GetHashCode() => (_value != null ? _value.GetHashCode() : 0);

        public static bool operator ==(SemanticTokenFormat left, SemanticTokenFormat right) => left.Equals(right);

        public static bool operator !=(SemanticTokenFormat left, SemanticTokenFormat right) => !left.Equals(right);
    }
}
