using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonConverter(typeof(TextEditConverter))]
    [GenerateContainer]
    public record TextEdit
    {
        /// <summary>
        /// The range of the text document to be manipulated. To insert
        /// text into a document create a range where start === end.
        /// </summary>
        public Range Range { get; init; } = null!;

        /// <summary>
        /// The string to be inserted. For delete operations use an
        /// empty string.
        /// </summary>
        public string NewText { get; init; } = null!;

        private string DebuggerDisplay =>
            $"{Range} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }

    /// <summary>
    /// A special text edit to provide an insert and a replace operation.
    ///
    /// @since 3.16.0
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record InsertReplaceEdit
    {
        /// <summary>
        /// The string to be inserted.
        /// </summary>
        public string NewText { get; init; } = null!;

        /// <summary>
        /// The range if the insert is requested
        /// </summary>
        public Range Insert { get; init; } = null!;

        /// <summary>
        /// The range if the replace is requested.
        /// </summary>
        public Range Replace { get; init; } = null!;

        private string DebuggerDisplay =>
            $"{Insert} / {Replace} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }

    [JsonConverter(typeof(TextEditOrInsertReplaceEditConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [GenerateContainer]
    public record TextEditOrInsertReplaceEdit
    {
        private TextEdit? _textEdit;
        private InsertReplaceEdit? _insertReplaceEdit;

        public TextEditOrInsertReplaceEdit(TextEdit value)
        {
            _textEdit = value;
            _insertReplaceEdit = default;
        }

        public TextEditOrInsertReplaceEdit(InsertReplaceEdit value)
        {
            _textEdit = default;
            _insertReplaceEdit = value;
        }

        public bool IsInsertReplaceEdit => _insertReplaceEdit != null;

        public InsertReplaceEdit? InsertReplaceEdit
        {
            get => _insertReplaceEdit;
            set
            {
                _insertReplaceEdit = value;
                _textEdit = null;
            }
        }

        public bool IsTextEdit => _textEdit != null;

        public TextEdit? TextEdit
        {
            get => _textEdit;
            set
            {
                _insertReplaceEdit = default;
                _textEdit = value;
            }
        }

        public object? RawValue
        {
            get
            {
                if (IsTextEdit) return TextEdit!;
                if (IsInsertReplaceEdit) return InsertReplaceEdit!;
                return default;
            }
        }

        public static TextEditOrInsertReplaceEdit From(TextEdit value)
        {
            return new(value);
        }

        public static implicit operator TextEditOrInsertReplaceEdit(TextEdit value)
        {
            return new(value);
        }

        public static TextEditOrInsertReplaceEdit From(InsertReplaceEdit value)
        {
            return new(value);
        }

        public static implicit operator TextEditOrInsertReplaceEdit(InsertReplaceEdit value)
        {
            return new(value);
        }

        private string DebuggerDisplay => $"{( IsInsertReplaceEdit ? $"insert: {InsertReplaceEdit}" : IsTextEdit ? $"edit: {TextEdit}" : "..." )}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }

    /// <summary>
    /// Additional information that describes document changes.
    ///
    /// @since 3.16.0
    /// </summary>
    public record ChangeAnnotation
    {
        /// <summary>
        /// A human-readable string describing the actual change. The string
        /// is rendered prominent in the user interface.
        /// </summary>
        public string Label { get; init; } = null!;

        /// <summary>
        /// A flag which indicates that user confirmation is needed
        /// before applying the change.
        /// </summary>
        [Optional]
        public bool NeedsConfirmation { get; init; }

        /// <summary>
        /// A human-readable string which is rendered less prominent in
        /// the user interface.
        /// </summary>
        [Optional]
        public string? Description { get; init; }
    }

    public record ChangeAnnotationIdentifier
    {
        /// <summary>
        /// An optional annotation identifer describing the operation.
        ///
        /// @since 3.16.0
        /// </summary>
        public string Identifier { get; init; } = null!;

        public static implicit operator string(ChangeAnnotationIdentifier identifier)
        {
            return identifier.Identifier;
        }

        public static implicit operator ChangeAnnotationIdentifier(string identifier)
        {
            return new() { Identifier = identifier };
        }
    }

    /// <summary>
    /// A special text edit with an additional change annotation.
    ///
    /// @since 3.16.0.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonConverter(typeof(TextEditConverter))]
    public record AnnotatedTextEdit : TextEdit
    {
        /// <summary>
        /// The actual annotation
        /// </summary>
        public ChangeAnnotationIdentifier AnnotationId { get; init; } = null!;

        private string DebuggerDisplay =>
            $"annotationId: {Range} {( string.IsNullOrWhiteSpace(NewText) ? string.Empty : NewText.Length > 30 ? NewText.Substring(0, 30) : NewText )}";

        /// <inheritdoc />
        public override string ToString()
        {
            return DebuggerDisplay;
        }
    }
}
