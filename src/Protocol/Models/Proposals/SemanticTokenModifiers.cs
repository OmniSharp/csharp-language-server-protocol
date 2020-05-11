using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// A set of predefined token modifiers. This set is not fixed
    /// an clients can specify additional token types via the
    /// corresponding client capabilities.
    ///
    /// @since 3.16.0
    /// </summary>
    [JsonConverter(typeof(EnumLikeStringConverter))]
    [Obsolete(Constants.Proposal)]
    [DebuggerDisplay("{_value}")]
    public struct SemanticTokenModifier : IEquatable<SemanticTokenModifier>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<SemanticTokenModifier>> _defaults =
            new Lazy<IReadOnlyList<SemanticTokenModifier>>(
                () => {
                    return typeof(SemanticTokenModifier)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(z => z.GetValue(null))
                        .Cast<SemanticTokenModifier>()
                        .ToArray();
                });

        public static IEnumerable<SemanticTokenModifier> Defaults => _defaults.Value;

        public static readonly SemanticTokenModifier Documentation = new SemanticTokenModifier("documentation");
        public static readonly SemanticTokenModifier Declaration = new SemanticTokenModifier("declaration");
        public static readonly SemanticTokenModifier Definition = new SemanticTokenModifier("definition");
        public static readonly SemanticTokenModifier Static = new SemanticTokenModifier("static");
        public static readonly SemanticTokenModifier Abstract = new SemanticTokenModifier("abstract");
        public static readonly SemanticTokenModifier Deprecated = new SemanticTokenModifier("deprecated");
        public static readonly SemanticTokenModifier Readonly = new SemanticTokenModifier("readonly");

        private readonly string _value;

        public SemanticTokenModifier(string modifier)
        {
            _value = modifier;
        }

        public static implicit operator SemanticTokenModifier(string kind)
        {
            return new SemanticTokenModifier(kind);
        }

        public static implicit operator string(SemanticTokenModifier kind)
        {
            return kind._value;
        }

        public override string ToString() => _value;
        public bool Equals(SemanticTokenModifier other) => _value == other._value;

        public override bool Equals(object obj) => obj is SemanticTokenModifier other && Equals(other);

        public override int GetHashCode() => (_value != null ? _value.GetHashCode() : 0);

        public static bool operator ==(SemanticTokenModifier left, SemanticTokenModifier right) => left.Equals(right);

        public static bool operator !=(SemanticTokenModifier left, SemanticTokenModifier right) => !left.Equals(right);
    }
}
