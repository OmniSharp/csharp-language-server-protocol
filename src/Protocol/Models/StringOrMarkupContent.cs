using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(StringOrMarkupContentConverter))]
    public class StringOrMarkupContent
    {
        public StringOrMarkupContent(string value)
        {
            String = value;
        }

        public StringOrMarkupContent(MarkupContent markupContent)
        {
            MarkupContent = markupContent;
        }

        public string String { get; }
        public bool HasString => this.MarkupContent == null;
        public MarkupContent MarkupContent { get; }
        public bool HasMarkupContent => String == null;
    }
}
