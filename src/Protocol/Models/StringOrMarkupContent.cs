using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringOrMarkupContentConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record StringOrMarkupContent
    {
        public StringOrMarkupContent(string value) => String = value;

        public StringOrMarkupContent(MarkupContent markupContent) => MarkupContent = markupContent;

        public string? String { get; }
        public bool HasString => MarkupContent == null;
        public MarkupContent? MarkupContent { get; }
        public bool HasMarkupContent => String == null;

        public static implicit operator StringOrMarkupContent(string value) => new StringOrMarkupContent(value);

        public static implicit operator StringOrMarkupContent(MarkupContent markupContent) => new StringOrMarkupContent(markupContent);

        private string DebuggerDisplay => $"{( HasString ? String : HasMarkupContent ? MarkupContent!.ToString() : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
