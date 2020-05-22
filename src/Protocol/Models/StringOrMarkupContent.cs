namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
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

        public static implicit operator StringOrMarkupContent(string value)
        {
            return new StringOrMarkupContent(value);
        }

        public static implicit operator StringOrMarkupContent(MarkupContent markupContent)
        {
            return new StringOrMarkupContent(markupContent);
        }
    }
}
