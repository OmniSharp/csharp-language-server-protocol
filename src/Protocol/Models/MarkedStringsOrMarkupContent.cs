using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(MarkedStringsOrMarkupContentConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record MarkedStringsOrMarkupContent
    {
        public MarkedStringsOrMarkupContent(params MarkedString[] markedStrings) => MarkedStrings = markedStrings;

        public MarkedStringsOrMarkupContent(IEnumerable<MarkedString> markedStrings) => MarkedStrings = markedStrings.ToArray();

        public MarkedStringsOrMarkupContent(MarkupContent markupContent) => MarkupContent = markupContent;

        public Container<MarkedString>? MarkedStrings { get; }
        public bool HasMarkedStrings => MarkupContent is null;
        public MarkupContent? MarkupContent { get; }
        public bool HasMarkupContent => MarkedStrings is null;

        private string DebuggerDisplay =>
            $"{( HasMarkedStrings ? string.Join(" ", MarkedStrings!.Select(z => z.ToString())) : HasMarkupContent ? MarkupContent!.ToString() : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
