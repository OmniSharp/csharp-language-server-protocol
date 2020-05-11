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
    /// A set of predefined token types. This set is not fixed
    /// an clients can specify additional token types via the
    /// corresponding client capabilities.
    ///
    /// @since 3.16.0
    /// </summary>
    [JsonConverter(typeof(EnumLikeStringConverter))]
    [Obsolete(Constants.Proposal)]
    [DebuggerDisplay("{_value}")]
    public struct SemanticTokenType : IEquatable<SemanticTokenType>, IEnumLikeString
    {
        private static readonly Lazy<IReadOnlyList<SemanticTokenType>> _defaults =
            new Lazy<IReadOnlyList<SemanticTokenType>>(
                () => {
                    return typeof(SemanticTokenType)
                        .GetFields(BindingFlags.Static | BindingFlags.Public)
                        .Select(z => z.GetValue(null))
                        .Cast<SemanticTokenType>()
                        .ToArray();
                });

        public static IEnumerable<SemanticTokenType> Defaults => _defaults.Value;

        public static readonly SemanticTokenType Documentation = new SemanticTokenType("documentation");
        public static readonly SemanticTokenType Comment = new SemanticTokenType("comment");
        public static readonly SemanticTokenType Keyword = new SemanticTokenType("keyword");
        public static readonly SemanticTokenType String = new SemanticTokenType("string");
        public static readonly SemanticTokenType Number = new SemanticTokenType("number");
        public static readonly SemanticTokenType Regexp = new SemanticTokenType("regexp");
        public static readonly SemanticTokenType Operator = new SemanticTokenType("operator");
        public static readonly SemanticTokenType Namespace = new SemanticTokenType("namespace");
        public static readonly SemanticTokenType Type = new SemanticTokenType("type");
        public static readonly SemanticTokenType Struct = new SemanticTokenType("struct");
        public static readonly SemanticTokenType Class = new SemanticTokenType("class");
        public static readonly SemanticTokenType Interface = new SemanticTokenType("interface");
        public static readonly SemanticTokenType Enum = new SemanticTokenType("enum");
        public static readonly SemanticTokenType TypeParameter = new SemanticTokenType("typeParameter");
        public static readonly SemanticTokenType Function = new SemanticTokenType("function");
        public static readonly SemanticTokenType Member = new SemanticTokenType("member");
        public static readonly SemanticTokenType Property = new SemanticTokenType("property");
        public static readonly SemanticTokenType Macro = new SemanticTokenType("macro");
        public static readonly SemanticTokenType Variable = new SemanticTokenType("variable");
        public static readonly SemanticTokenType Parameter = new SemanticTokenType("parameter");
        public static readonly SemanticTokenType Label = new SemanticTokenType("label");

        private readonly string _value;

        public SemanticTokenType(string type)
        {
            _value = type;
        }

        public static implicit operator SemanticTokenType(string kind)
        {
            return new SemanticTokenType(kind);
        }

        public static implicit operator string(SemanticTokenType kind)
        {
            return kind._value;
        }

        public override string ToString() => _value;
        public bool Equals(SemanticTokenType other) => _value == other._value;

        public override bool Equals(object obj) => obj is SemanticTokenType other && Equals(other);

        public override int GetHashCode() => (_value != null ? _value.GetHashCode() : 0);

        public static bool operator ==(SemanticTokenType left, SemanticTokenType right) => left.Equals(right);

        public static bool operator !=(SemanticTokenType left, SemanticTokenType right) => !left.Equals(right);
    }
}
