namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class PlaceholderRange
    {
        public Range Range { get; set; } = null!;
        public string Placeholder { get; set; } = null!;
    }
}
