using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof( StringOrMarkupContentConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record StringOrMarkupContent
    {
        public StringOrMarkupContent(string value) => String = value;

        public StringOrMarkupContent(MarkupContent markupContent) => MarkupContent = markupContent;

        public string? String { get; }
        public bool HasString => MarkupContent == null;
        public MarkupContent? MarkupContent { get; }
        public bool HasMarkupContent => String == null;

        [return: NotNullIfNotNull("value")]
        public static implicit operator StringOrMarkupContent?(string? value) => value is null ? null : new StringOrMarkupContent(value);

        [return: NotNullIfNotNull("markupContent")]
        public static implicit operator StringOrMarkupContent?(MarkupContent? markupContent) => markupContent is null ? null : new StringOrMarkupContent(markupContent);

        private string DebuggerDisplay => $"{( HasString ? String : HasMarkupContent ? MarkupContent!.ToString() : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
