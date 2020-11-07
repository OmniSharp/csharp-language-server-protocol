namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public record PlaceholderRange(Range Range, string Placeholder)
    {
        public PlaceholderRange(): this(( ( 0, 0 ), ( 0, 0 ) ), "") { }
    }
}
