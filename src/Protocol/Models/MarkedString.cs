using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// MarkedString can be used to render human readable text. It is either a markdown string
    /// or a code-block that provides a language and a code snippet. The language identifier
    /// is sematically equal to the optional language identifier in fenced code blocks in GitHub
    /// issues. See https://help.github.com/articles/creating-and-highlighting-code-blocks/#syntax-highlighting
    ///
    /// The pair of a language and a value is an equivalent to markdown:
    /// ```${language}
    /// ${value}
    /// ```
    ///
    /// Note that markdown strings will be sanitized - that means html will be escaped.
    /// </summary>
    public class MarkedString
    {
        public MarkedString(string value)
        {
            Value = value;
        }

        public MarkedString(string language, string value) : this(value)
        {
            Language = language;
        }

        [Optional]
        public string Language { get; }

        public string Value { get; }

        public static implicit operator MarkedString(string value)
        {
            return new MarkedString(value);
        }
    }
}
