using System;
using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// A set of predefined code action kinds
    /// </summary>
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct UniquenessLevel : IEquatable<UniquenessLevel>, IEnumLikeString
    {
        /// <summary>
        /// The moniker is only unique inside a document
        /// </summary>
        public static readonly UniquenessLevel Document = new UniquenessLevel("document");

        /// <summary>
        /// The moniker is unique inside a project for which a dump got created
        /// </summary>
        public static readonly UniquenessLevel Project = new UniquenessLevel("project");

        /// <summary>
        /// The moniker is unique inside the group to which a project belongs
        /// </summary>
        public static readonly UniquenessLevel Group = new UniquenessLevel("group");

        /// <summary>
        /// The moniker is unique inside the moniker scheme.
        /// </summary>
        public static readonly UniquenessLevel Scheme = new UniquenessLevel("scheme");

        /// <summary>
        /// The moniker is globally unique
        /// </summary>
        public static readonly UniquenessLevel Global = new UniquenessLevel("global");

        private readonly string? _value;

        public UniquenessLevel(string kind) => _value = kind;

        public static implicit operator UniquenessLevel(string kind) => new UniquenessLevel(kind);

        public static implicit operator string(UniquenessLevel kind) => kind._value ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => _value ?? string.Empty;

        public bool Equals(UniquenessLevel other) => _value == other._value;

        public override bool Equals(object obj) => obj is UniquenessLevel other && Equals(other);

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(UniquenessLevel left, UniquenessLevel right) => left.Equals(right);

        public static bool operator !=(UniquenessLevel left, UniquenessLevel right) => !left.Equals(right);
    }
}