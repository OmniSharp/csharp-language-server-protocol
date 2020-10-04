using System;
using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Moniker uniqueness level to define scope of the moniker.
    /// </summary>
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct MonikerKind : IEquatable<MonikerKind>, IEnumLikeString
    {
        /// <summary>
        /// The moniker represent a symbol that is imported into a project
        /// </summary>
        public static readonly MonikerKind Import = new MonikerKind("import");

        /// <summary>
        /// The moniker represents a symbol that is exported from a project
        /// </summary>
        public static readonly MonikerKind Export = new MonikerKind("export");

        /// <summary>
        /// The moniker represents a symbol that is local to a project (e.g. a local
        /// variable of a function, a class not visible outside the project, ...)
        /// </summary>
        public static readonly MonikerKind Local = new MonikerKind("local");

        private readonly string? _value;

        public MonikerKind(string kind) => _value = kind;

        public static implicit operator MonikerKind(string kind) => new MonikerKind(kind);

        public static implicit operator string(MonikerKind kind) => kind._value ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => _value ?? string.Empty;

        public bool Equals(MonikerKind other) => _value == other._value;

        public override bool Equals(object obj) => obj is MonikerKind other && Equals(other);

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(MonikerKind left, MonikerKind right) => left.Equals(right);

        public static bool operator !=(MonikerKind left, MonikerKind right) => !left.Equals(right);
    }
}