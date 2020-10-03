using System;
using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// A set of predefined code action kinds
    /// </summary>
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct CodeActionKind : IEquatable<CodeActionKind>, IEnumLikeString
    {
        /// <summary>
        /// Base kind for quickfix actions: ''
        /// </summary>
        public static readonly CodeActionKind Empty = new CodeActionKind("");

        /// <summary>
        /// Base kind for quickfix actions: 'quickfix'
        /// </summary>
        public static readonly CodeActionKind QuickFix = new CodeActionKind("quickfix");

        /// <summary>
        /// Base kind for refactoring actions: 'refactor'
        /// </summary>
        public static readonly CodeActionKind Refactor = new CodeActionKind("refactor");

        /// <summary>
        /// Base kind for refactoring extraction actions: 'refactor.extract'
        ///
        /// Example extract actions:
        ///
        /// - Extract method
        /// - Extract function
        /// - Extract variable
        /// - Extract interface from class
        /// - ...
        /// </summary>
        public static readonly CodeActionKind RefactorExtract = new CodeActionKind("refactor.extract");

        /// <summary>
        /// Base kind for refactoring inline actions: 'refactor.inline'
        ///
        /// Example inline actions:
        ///
        /// - Inline function
        /// - Inline variable
        /// - Inline constant
        /// - ...
        /// </summary>
        public static readonly CodeActionKind RefactorInline = new CodeActionKind("refactor.inline");

        /// <summary>
        /// Base kind for refactoring rewrite actions: 'refactor.rewrite'
        ///
        /// Example rewrite actions:
        ///
        /// - Convert JavaScript function to class
        /// - Add or remove parameter
        /// - Encapsulate field
        /// - Make method static
        /// - Move method to base class
        /// - ...
        /// </summary>
        public static readonly CodeActionKind RefactorRewrite = new CodeActionKind("refactor.rewrite");

        /// <summary>
        /// Base kind for source actions: `source`
        ///
        /// Source code actions apply to the entire file.
        /// </summary>
        public static readonly CodeActionKind Source = new CodeActionKind("source");

        /// <summary>
        /// Base kind for an organize imports source action: `source.organizeImports`
        /// </summary>
        public static readonly CodeActionKind SourceOrganizeImports = new CodeActionKind("source.organizeImports");

        private readonly string? _value;

        public CodeActionKind(string kind) => _value = kind;

        public static implicit operator CodeActionKind(string kind) => new CodeActionKind(kind);

        public static implicit operator string(CodeActionKind kind) => kind._value ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => _value ?? string.Empty;

        public bool Equals(CodeActionKind other) => _value == other._value;

        public override bool Equals(object obj) => obj is CodeActionKind other && Equals(other);

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(CodeActionKind left, CodeActionKind right) => left.Equals(right);

        public static bool operator !=(CodeActionKind left, CodeActionKind right) => !left.Equals(right);
    }

    public interface IEnumLikeString
    {
    }
}
