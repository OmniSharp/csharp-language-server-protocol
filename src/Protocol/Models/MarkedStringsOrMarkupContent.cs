using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class MarkedStringsOrMarkupContent
    {
        public MarkedStringsOrMarkupContent(params MarkedString[] markedStrings)
        {
            MarkedStrings = markedStrings;
        }

        public MarkedStringsOrMarkupContent(IEnumerable<MarkedString> markedStrings)
        {
            MarkedStrings = markedStrings.ToArray();
        }

        public MarkedStringsOrMarkupContent(MarkupContent markupContent)
        {
            MarkupContent = markupContent;
        }

        public MarkedStringContainer MarkedStrings { get; }
        public bool HasMarkedStrings => this.MarkupContent == null;
        public MarkupContent MarkupContent { get; }
        public bool HasMarkupContent => MarkedStrings == null;
    }
}
