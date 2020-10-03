namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public struct DocumentUriComponents
    {
        public string? Scheme { get; set; }
        public string? Authority { get; set; }
        public string? Path { get; set; }
        public string? Query { get; set; }
        public string? Fragment { get; set; }
    }
}
